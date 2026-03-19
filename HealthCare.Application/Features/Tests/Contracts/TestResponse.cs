using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.Tests.Contracts;

public record TestResponse(
    Guid Id,
    string Name,
    string Description,
    string PreRequisites
);
