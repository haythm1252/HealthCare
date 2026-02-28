using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.Auth.Contracts;

public record AuthResponse(
    string Id,
    string Email,
    string Name,
    string Role,
    string Token,
    int ExpireIn,
    string RefreshToken,
    DateTime RefreshTokenExpiration
);
