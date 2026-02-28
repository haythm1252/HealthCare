using System.ComponentModel.DataAnnotations;

namespace HealthCare.Application.Common.Settings;

public class MailSettings
{
    public const string SectionName = "MailSettings";
    [EmailAddress]
    [Required]
    public string Email { get; set; } = string.Empty;
    [Required]
    public string DisplayName { get; set; } = string.Empty;
    [Required]
    public string Password { get; set; } = string.Empty;
    [Required]
    public string SmtpServer { get; set; } = string.Empty;
    [Required]
    public int Port { get; set; }
}
