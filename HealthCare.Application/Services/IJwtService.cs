using HealthCare.Domain.Users;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Services;

public interface IJwtService
{
    (string token, int expireIn) GenerateToken(ApplicationUser user, string role);
    string? ValidateToken(string token);
}
