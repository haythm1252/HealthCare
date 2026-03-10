using HealthCare.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Domain.Users;

public class ApplicationUser : IdentityUser
{
    public string Name { get; set; } = string.Empty;
    public Gender? Gender { get; set; }
    public string Address { get; set; } = string.Empty;
    public string? AddressUrl { get; set; }
    public string City { get; set; } = string.Empty;
    public bool IsDisabled { get; set; } = false;
    public DateTime? LastLogin { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public ICollection<RefreshToken> RefreshTokens { get; set; } = [];

}
