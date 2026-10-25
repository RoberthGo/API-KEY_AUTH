using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
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
    public class AuthController : ControllerBase
    {
        private readonly ContextDB _context;

        public AuthController(ContextDB context)
        {
            _context = context;
        }

        // POST: api/auth/register
        // Registra un nuevo usuario y le genera su primera API Key
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<ActionResult<AuthResponseDto>> PostRegister(RegisterDto dto)
        {
            var usernameExists = await _context.Users
                .AnyAsync(u => u.Username.ToLower() == dto.Username.Trim().ToLower());

            if (usernameExists)
            {
                return Problem(
                    statusCode: StatusCodes.Status400BadRequest,
                    title: "Usuario duplicado",
                    detail: $"El nombre de usuario '{dto.Username}' ya se encuentra registrado."
                );
            }

            var emailExists = await _context.Users
                .AnyAsync(u => u.Email.ToLower() == dto.Email.Trim().ToLower());

            if (emailExists)
            {
                return Problem(
                    statusCode: StatusCodes.Status400BadRequest,
                    title: "Correo duplicado",
                    detail: $"El correo electrónico '{dto.Email}' ya se encuentra registrado."
                );
            }

            try
            {
                // 1. Crear el usuario
                var user = new User
                {
                    Username = dto.Username.Trim(),
                    Email = dto.Email.Trim().ToLowerInvariant(),
                    PasswordHash = HashPassword(dto.Password),
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                // 2. Generar su primera API Key reutilizando ApiKeysController
                var rawApiKey = ApiKeysController.GenerateApiKey();
                var createdAt = DateTime.UtcNow;
                var expiresAt = createdAt.AddDays(30);

                var apiKeyEntity = new ApiKey
                {
                    ClientIdentifier = user.Username,
                    KeyHash = ApiKeysController.HashApiKey(rawApiKey),
                    CreatedAt = createdAt,
                    ExpiresAt = expiresAt,
                    IsActive = true
                };

                _context.ApiKeys.Add(apiKeyEntity);
                await _context.SaveChangesAsync();

                return StatusCode(StatusCodes.Status201Created, new AuthResponseDto
                {
                    UserId = user.Id,
                    Username = user.Username,
                    ApiKey = rawApiKey,
                    ExpiresAt = expiresAt
                });
            }
            catch (DbUpdateException ex)
            {
                return Problem(
                    statusCode: StatusCodes.Status400BadRequest,
                    title: "Error al registrar usuario",
                    detail: ex.InnerException?.Message ?? ex.Message
                );
            }
        }

        // POST: api/auth/login
        // Valida credenciales y genera una nueva API Key para el usuario
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<AuthResponseDto>> PostLogin(LoginDto dto)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username.ToLower() == dto.Username.Trim().ToLower() && u.IsActive);

            if (user == null || !VerifyPassword(dto.Password, user.PasswordHash))
            {
                return Problem(
                    statusCode: StatusCodes.Status401Unauthorized,
                    title: "Credenciales inválidas",
                    detail: "El usuario o la contraseña proporcionados son incorrectos."
                );
            }

            try
            {
                var rawApiKey = ApiKeysController.GenerateApiKey();
                var createdAt = DateTime.UtcNow;
                var expiresAt = createdAt.AddDays(30);

                var apiKeyEntity = new ApiKey
                {
                    ClientIdentifier = user.Username,
                    KeyHash = ApiKeysController.HashApiKey(rawApiKey),
                    CreatedAt = createdAt,
                    ExpiresAt = expiresAt,
                    IsActive = true
                };

                _context.ApiKeys.Add(apiKeyEntity);
                await _context.SaveChangesAsync();

                return Ok(new AuthResponseDto
                {
                    UserId = user.Id,
                    Username = user.Username,
                    ApiKey = rawApiKey,
                    ExpiresAt = expiresAt
                });
            }
            catch (DbUpdateException ex)
            {
                return Problem(
                    statusCode: StatusCodes.Status400BadRequest,
                    title: "Error al generar API Key",
                    detail: ex.InnerException?.Message ?? ex.Message
                );
            }
        }

        // ==========================================
        // Métodos Auxiliares de Seguridad (Contraseñas)
        // ==========================================

        private static string HashPassword(string password)
        {
            byte[] salt = RandomNumberGenerator.GetBytes(16);
            byte[] hash = Rfc2898DeriveBytes.Pbkdf2(
                Encoding.UTF8.GetBytes(password),
                salt,
                iterations: 100_000,
                hashAlgorithm: HashAlgorithmName.SHA256,
                outputLength: 32);

            return $"{Convert.ToHexString(salt)}:{Convert.ToHexString(hash)}";
        }

        private static bool VerifyPassword(string password, string storedHash)
        {
            var parts = storedHash.Split(':');
            if (parts.Length != 2) return false;

            byte[] salt = Convert.FromHexString(parts[0]);
            byte[] expectedHash = Convert.FromHexString(parts[1]);

            byte[] actualHash = Rfc2898DeriveBytes.Pbkdf2(
                Encoding.UTF8.GetBytes(password),
                salt,
                iterations: 100_000,
                hashAlgorithm: HashAlgorithmName.SHA256,
                outputLength: 32);

            return CryptographicOperations.FixedTimeEquals(actualHash, expectedHash);
        }
    }
}
