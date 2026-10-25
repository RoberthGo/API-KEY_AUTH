using System;
using System.ComponentModel.DataAnnotations;

namespace APIkey.DTOs;

public class CreateApiKeyDto
{
    [Required]
    [StringLength(200)]
    public string ClientIdentifier { get; set; } = string.Empty;

    [Range(1, 3650)]
    public int ValidityDays { get; set; } = 30;
}

public class ApiKeySummaryDto
{
    public int Id { get; set; }

    public string ClientIdentifier { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public bool IsActive { get; set; }

    public DateTime? ExpiresAt { get; set; }    
}

public class RevokeApiKeyDto
{
    [Required]
    public string Password { get; set; } = string.Empty;
}
