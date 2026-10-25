using System;
using System.ComponentModel.DataAnnotations;

namespace APIkey.DTOs;

// POST /api/auth/register
public class RegisterDto
{
    [Required]
    public string Username { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MinLength(8)]
    public string Password { get; set; } = string.Empty;
}

// POST /api/auth/login
public class LoginDto
{
    [Required]
    public string Username { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;
}

// Respuesta de Login con la API Key generada
public class AuthResponseDto
{
    public int UserId { get; set; }

    public string Username { get; set; } = string.Empty;

    public string ApiKey { get; set; } = string.Empty;

    public DateTime? ExpiresAt { get; set; }
}