using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using APIkey.Data;
using APIkey.DTOs;
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
