using System;

namespace APIkey.Models;

public class ApiKey
{
    public int Id { get; set; }

    public string ClientIdentifier { get; set; } = string.Empty;

    public string KeyHash { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public bool IsActive { get; set; } = true;

    public DateTime? ExpiresAt { get; set; }
}
