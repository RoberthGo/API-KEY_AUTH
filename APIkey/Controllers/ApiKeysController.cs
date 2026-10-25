using System;
using System.Collections.Generic;
using System.Linq;
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

        // GET: api/apikeys/user/roberth
        // Retorna la lista de todas las API Keys asociadas a un usuario
        [HttpGet("user/{username}")]
        public async Task<ActionResult<IEnumerable<ApiKeySummaryDto>>> GetUserKeys(string username)
        {
            var userExists = await _context.Users
                .AnyAsync(u => u.Username.ToLower() == username.Trim().ToLower());

            if (!userExists)
            {
                return Problem(
                    statusCode: StatusCodes.Status404NotFound,
                    title: "Usuario no encontrado",
                    detail: $"No se encontró ningún usuario con el nombre '{username}'."
                );
            }

            var keys = await _context.ApiKeys
                .AsNoTracking()
                .Where(k => k.ClientIdentifier.ToLower() == username.Trim().ToLower())
                .OrderByDescending(k => k.CreatedAt)
                .Select(k => new ApiKeySummaryDto
                {
                    Id = k.Id,
                    ClientIdentifier = k.ClientIdentifier,
                    CreatedAt = k.CreatedAt,
                    IsActive = k.IsActive,
                    ExpiresAt = k.ExpiresAt
                })
                .ToListAsync();

            return Ok(keys);
        }

        // POST: api/apikeys
        // Crea y asocia una nueva API Key para un usuario existente
        [HttpPost]
        public async Task<ActionResult<ApiKeyCreatedResponseDto>> PostApiKey([FromBody] CreateApiKeyDto request)
        {
            if (string.IsNullOrWhiteSpace(request.ClientIdentifier))
            {
                return Problem(
                    statusCode: StatusCodes.Status400BadRequest,
                    title: "Dato requerido",
                    detail: "El identificador del usuario (ClientIdentifier) es obligatorio."
                );
            }

            var userExists = await _context.Users
                .AnyAsync(u => u.Username.ToLower() == request.ClientIdentifier.Trim().ToLower());

            if (!userExists)
            {
                return Problem(
                    statusCode: StatusCodes.Status404NotFound,
                    title: "Usuario no encontrado",
                    detail: $"No se encontró ningún usuario registrado con el nombre '{request.ClientIdentifier}'."
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

        // POST: api/apikeys/5/revoke
        // Revoca una API Key específica verificando la contraseña del dueño
        [HttpPost("{id}/revoke")]
        public async Task<IActionResult> RevokeApiKey(int id, [FromBody] RevokeApiKeyDto request)
        {
            var apiKey = await _context.ApiKeys.FindAsync(id);
            if (apiKey == null)
            {
                return Problem(
                    statusCode: StatusCodes.Status404NotFound,
                    title: "API Key no encontrada",
                    detail: $"No se encontró ninguna API Key con el identificador {id}."
                );
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username.ToLower() == apiKey.ClientIdentifier.ToLower());

            if (user == null || !AuthController.VerifyPassword(request.Password, user.PasswordHash))
            {
                return Problem(
                    statusCode: StatusCodes.Status401Unauthorized,
                    title: "Contraseña incorrecta",
                    detail: "La contraseña proporcionada es incorrecta. No se puede revocar la API Key."
                );
            }

            apiKey.IsActive = false;
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = $"La API Key con ID {id} ha sido revocada exitosamente.",
                id = apiKey.Id,
                isActive = apiKey.IsActive
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
