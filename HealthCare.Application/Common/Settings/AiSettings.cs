using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace HealthCare.Application.Common.Settings;

public class AiSettings
{
    public static string SectionName = "AiSettings";
    [Required]
    public string ApiKey { get; set; } = string.Empty;
    [Required]
    public string SystemPrompt { get; set; } = string.Empty;
}