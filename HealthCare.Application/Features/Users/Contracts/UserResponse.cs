using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.Users.Contracts;

public record UserResponse(
    string Id,
    string Name,
    string Email,
    string Role,
    bool IsDisabled
);
