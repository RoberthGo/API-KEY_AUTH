using System;
using System.Security.Cryptography;
using System.Text;
using APIkey.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace APIkey.Filters;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class ApiKeyAttribute : ActionFilterAttribute, IAsyncActionFilter
{
    private const string API_KEY_HEADER_NAME = "X-API-KEY";

    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        // Permitir solicitudes preflight de CORS
        if (HttpMethods.IsOptions(context.HttpContext.Request.Method))
        {
            await next();
            return;
        }

        if (!context.HttpContext.Request.Headers.TryGetValue(API_KEY_HEADER_NAME, out var extractedApiKey) ||
            string.IsNullOrWhiteSpace(extractedApiKey))
        {
            var problemDetails = new ProblemDetails
            {
                Status = StatusCodes.Status401Unauthorized,
                Title = "No autorizado",
                Detail = "No se proporcionó la API Key en la cabecera 'X-API-KEY'.",
                Instance = context.HttpContext.Request.Path
            };

            context.Result = new ObjectResult(problemDetails)
            {
                StatusCode = StatusCodes.Status401Unauthorized,
                ContentTypes = { "application/problem+json" }
            };
            return;
        }

        var providedApiKey = extractedApiKey.ToString().Trim();

        var appSettings = context.HttpContext.RequestServices.GetRequiredService<IConfiguration>();
        var configuredApiKey = appSettings.GetValue<string>("ApiKey") ?? appSettings.GetValue<string>("ApiSettings:ApiKey");

        if (!string.IsNullOrWhiteSpace(configuredApiKey) &&
            string.Equals(configuredApiKey.Trim(), providedApiKey, StringComparison.Ordinal))
        {
            await next();
            return;
        }

        var dbContext = context.HttpContext.RequestServices.GetRequiredService<ContextDB>();
        var providedApiKeyHash = HashApiKey(providedApiKey);
        var utcNow = DateTime.UtcNow;

        var exists = await dbContext.ApiKeys
            .AsNoTracking()
            .AnyAsync(k => k.KeyHash == providedApiKeyHash &&
                           k.IsActive &&
                           (k.ExpiresAt == null || k.ExpiresAt > utcNow));

        if (!exists)
        {
            var problemDetails = new ProblemDetails
            {
                Status = StatusCodes.Status401Unauthorized,
                Title = "No autorizado",
                Detail = "La API Key proporcionada no es válida o está expirada.",
                Instance = context.HttpContext.Request.Path
            };

            context.Result = new ObjectResult(problemDetails)
            {
                StatusCode = StatusCodes.Status401Unauthorized,
                ContentTypes = { "application/problem+json" }
            };
            return;
        }

        await next();
    }

    private static string HashApiKey(string plainTextKey)
    {
        using var sha256 = SHA256.Create();
        var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(plainTextKey));
        return Convert.ToHexString(hashBytes).ToLowerInvariant();
    }
}
