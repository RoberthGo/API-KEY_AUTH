using System;

namespace APIkey.DTOs;

public class ApiKeyCreatedResponseDto
{
    public int Id { get; set; }

    public string ClientIdentifier { get; set; } = string.Empty;

    public string ApiKey { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public bool IsActive { get; set; }

    public DateTime? ExpiresAt { get; set; }
}
