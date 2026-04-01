using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace HealthCare.Application.Common.Settings;

public class PaymobSettings
{
    public const string SectionName = "Paymob";

    [Required]
    public string ApiKey { get; set; } = string.Empty;

    [Required]
    public string SecretKey { get; set; } = string.Empty;

    [Required]
    public string PublicKey { get; set; } = string.Empty;

    [Required]
    public string HmacSecret { get; set; } = string.Empty;

    public string IntegrationId { get; set; } = string.Empty;
    public string Currency { get; set; } = "EGP";
}
