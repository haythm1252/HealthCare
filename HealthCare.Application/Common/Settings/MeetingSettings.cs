using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace HealthCare.Application.Common.Settings;

public class MeetingSettings
{
    public const string SectionName = "MeetingSettings";
    [Required]
    public string ApiKey { get; set; } = string.Empty;
}