using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace APIkey.Filters;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class ApiKeyAttribute : ActionFilterAttribute, IAsyncActionFilter
{
    private const string API_KEY_HEADER_NAME = "X-API-KEY";

    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        // Allow CORS preflight requests through if they reach the action filter
        if (HttpMethods.IsOptions(context.HttpContext.Request.Method))
        {
            await next();
            return;
        }

        if (!context.HttpContext.Request.Headers.TryGetValue(API_KEY_HEADER_NAME, out var extractedApiKey) ||
            string.IsNullOrWhiteSpace(extractedApiKey))
        {
            context.Result = new ContentResult()
            {
                StatusCode = StatusCodes.Status401Unauthorized,
                Content = "API Key no proporcionada."
            };
            return;
        }

        var appSettings = context.HttpContext.RequestServices.GetRequiredService<IConfiguration>();
        var apiKey = appSettings.GetValue<string>("ApiKey") ?? appSettings.GetValue<string>("ApiSettings:ApiKey");

        if (string.IsNullOrEmpty(apiKey) || !string.Equals(apiKey.Trim(), extractedApiKey.ToString().Trim(), StringComparison.Ordinal))
        {
            context.Result = new ContentResult()
            {
                StatusCode = StatusCodes.Status401Unauthorized,
                Content = "Unauthorized client."
            };
            return;
        }

        await next();
    }
}
