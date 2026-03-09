using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Domain.Entities;

public class EmailOtp
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public string UserId { get; set; } = string.Empty;
    public string OtpCode { get; set; } = string.Empty;
    public DateTime ExpirationTime { get; set; } = DateTime.UtcNow.AddMinutes(5);
}
