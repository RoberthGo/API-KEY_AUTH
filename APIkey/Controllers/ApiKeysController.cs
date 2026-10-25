using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using APIkey.Data;
using APIkey.DTOs;
using APIkey.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace APIkey.Controllers
{
    /// <summary>
    /// Endpoint para la gestión y creación de API Keys.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ApiKeysController : ControllerBase
    {
        private readonly ContextDB _context;

        public ApiKeysController(ContextDB context)
        {
            _context = context;
        }

        /// <summary>
        /// Crea una nueva API Key para un cliente o usuario.
        /// </summary>
        /// <remarks>
        /// Route: POST /api/apikeys
        /// Method: HttpPost
        /// Descripción: genera una clave segura, la almacena con hash y devuelve la versión legible solo una vez.
        /// </remarks>
        /// <param name="request">Datos del cliente, descripción y vigencia de la API Key.</param>
        /// <returns>La respuesta con la API Key generada y sus datos de control.</returns>
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<ApiKeyCreatedResponseDto>> Post([FromBody] CreateApiKeyDto request)
        {
            if (request is null)
            {
                return BadRequest("La solicitud es obligatoria.");
            }

            if (string.IsNullOrWhiteSpace(request.ClientIdentifier))
            {
                return BadRequest("El identificador del cliente es obligatorio.");
            }

            if (request.ValidityDays < 1 || request.ValidityDays > 3650)
            {
                return BadRequest("Los días de vigencia deben estar entre 1 y 3650.");
            }

            var generatedApiKey = GenerateApiKey();
            var createdAt = DateTime.UtcNow;
            var expiresAt = createdAt.AddDays(request.ValidityDays);

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

            var response = new ApiKeyCreatedResponseDto
            {
                Id = apiKeyEntity.Id,
                ClientIdentifier = apiKeyEntity.ClientIdentifier,
                ApiKey = generatedApiKey,
                CreatedAt = apiKeyEntity.CreatedAt,
                IsActive = apiKeyEntity.IsActive,
                ExpiresAt = apiKeyEntity.ExpiresAt
            };

            return Ok(response);
        }

        private static string GenerateApiKey()
        {
            var randomBytes = RandomNumberGenerator.GetBytes(32);
            return $"ak_{Convert.ToHexString(randomBytes).ToLowerInvariant()}";
        }

        private static string HashApiKey(string plainTextKey)
        {
            using var sha256 = SHA256.Create();
            var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(plainTextKey));
            return Convert.ToHexString(hashBytes).ToLowerInvariant();
        }
    }
}
