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
