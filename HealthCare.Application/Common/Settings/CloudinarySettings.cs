using System.ComponentModel.DataAnnotations;

namespace HealthCare.Application.Common.Settings;

public class CloudinarySettings
{
    public const string SectionName = "CloudinarySettings";
    [Required]
    public string CloudName { get; set; } = string.Empty;
    [Required]
    public string ApiKey { get; set; } = string.Empty;
    [Required]
    public string ApiSecret { get; set; } = string.Empty;
}
