using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.Auth.Contracts;

public record RegisterResponse(
    string UserId,
    string Email,
    string Role,
    string Message
);
