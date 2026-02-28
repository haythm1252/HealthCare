using System.ComponentModel.DataAnnotations;

namespace HealthCare.Application.Common.Settings;

public class JwtSettings
{
    public const string SectionName = "JwtSettings";

    [Required]
    public string Key { get; set; } = string.Empty;

    [Required]
    public string Issuer { get; set; } = string.Empty;

    [Required]
    public string Audience { get; set; } = string.Empty;

    [Range(1, 1440)]
    [Required]
    public int ExpiryMinutes { get; set; }

    [Required]
    public int RefreshTokenExpiryDays { get; set; }
}
