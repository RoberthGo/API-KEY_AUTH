using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using APIkey.Data;
using APIkey.DTOs;
using APIkey.Filters;
using APIkey.Models;

namespace APIkey.Controllers
{
    [EnableCors("AllowAll")]
    [Route("api/[controller]")]
    [ApiController]
    public class ApiKeysController : ControllerBase
    {
        private readonly ContextDB _context;

        public ApiKeysController(ContextDB context)
        {
            _context = context;
        }

        // POST: api/apikeys
        // Crea una API Key para un cliente o usuario
        [HttpPost]
        public async Task<ActionResult<ApiKeyCreatedResponseDto>> Post([FromBody] CreateApiKeyDto request)
        {
            if (string.IsNullOrWhiteSpace(request.ClientIdentifier))
            {
                return Problem(
                    statusCode: StatusCodes.Status400BadRequest,
                    title: "Dato requerido",
                    detail: "El identificador del cliente es obligatorio."
                );
            }

            var generatedApiKey = GenerateApiKey();
            var createdAt = DateTime.UtcNow;
            var validityDays = request.ValidityDays > 0 ? request.ValidityDays : 30;
            var expiresAt = createdAt.AddDays(validityDays);

            var apiKeyEntity = new ApiKey
            {
                ClientIdentifier = request.ClientIdentifier.Trim(),
                KeyHash = HashApiKey(generatedApiKey),
                CreatedAt = createdAt,
                IsActive = true,
                ExpiresAt = expiresAt
            };

            _context.ApiKeys.Add(apiKeyEntity);
            await _context.SaveChangesAsync();

            return StatusCode(StatusCodes.Status201Created, new ApiKeyCreatedResponseDto
            {
                Id = apiKeyEntity.Id,
                ClientIdentifier = apiKeyEntity.ClientIdentifier,
                ApiKey = generatedApiKey,
                CreatedAt = apiKeyEntity.CreatedAt,
                IsActive = apiKeyEntity.IsActive,
                ExpiresAt = apiKeyEntity.ExpiresAt
            });
        }

        // POST: api/apikeys/regenerate
        // Invalida la API Key actual y genera una nueva para el mismo usuario
        [HttpPost("regenerate")]
        [ApiKey]
        public async Task<ActionResult<ApiKeyCreatedResponseDto>> PostRegenerateApiKey()
        {
            if (!Request.Headers.TryGetValue("X-API-KEY", out var currentKeyHeader))
            {
                return Problem(
                    statusCode: StatusCodes.Status401Unauthorized,
                    title: "No autorizado",
                    detail: "No se proporcionó la API Key en la cabecera 'X-API-KEY'."
                );
            }

            var currentKey = currentKeyHeader.ToString().Trim();
            var currentKeyHash = HashApiKey(currentKey);

            var currentApiKeyEntity = await _context.ApiKeys
                .FirstOrDefaultAsync(k => k.KeyHash == currentKeyHash && k.IsActive);

            if (currentApiKeyEntity == null)
            {
                return Problem(
                    statusCode: StatusCodes.Status401Unauthorized,
                    title: "No autorizado",
                    detail: "La API Key proporcionada no es válida o está inactiva."
                );
            }

            try
            {
                // 1. Desactivar la llave actual (darla de baja al regenerar)
                currentApiKeyEntity.IsActive = false;

                // 2. Generar y guardar la nueva API Key para el mismo usuario
                var newRawApiKey = GenerateApiKey();
                var createdAt = DateTime.UtcNow;
                var expiresAt = createdAt.AddDays(30);

                var newApiKeyEntity = new ApiKey
                {
                    ClientIdentifier = currentApiKeyEntity.ClientIdentifier,
                    KeyHash = HashApiKey(newRawApiKey),
                    CreatedAt = createdAt,
                    ExpiresAt = expiresAt,
                    IsActive = true
                };

                _context.ApiKeys.Add(newApiKeyEntity);
                await _context.SaveChangesAsync();

                return Ok(new ApiKeyCreatedResponseDto
                {
                    Id = newApiKeyEntity.Id,
                    ClientIdentifier = newApiKeyEntity.ClientIdentifier,
                    ApiKey = newRawApiKey,
                    CreatedAt = createdAt,
                    IsActive = true,
                    ExpiresAt = expiresAt
                });
            }
            catch (DbUpdateException ex)
            {
                return Problem(
                    statusCode: StatusCodes.Status400BadRequest,
                    title: "Error al regenerar API Key",
                    detail: ex.InnerException?.Message ?? ex.Message
                );
            }
        }

        // ==========================================
        // Métodos Auxiliares Compartidos
        // ==========================================

        public static string GenerateApiKey()
        {
            var randomBytes = RandomNumberGenerator.GetBytes(32);
            return $"ak_{Convert.ToHexString(randomBytes).ToLowerInvariant()}";
        }

        public static string HashApiKey(string plainTextKey)
        {
            using var sha256 = SHA256.Create();
            var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(plainTextKey));
            return Convert.ToHexString(hashBytes).ToLowerInvariant();
        }
    }
}
