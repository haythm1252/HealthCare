using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.Tests.Contracts;

public record UpdateTestRequest(
    string Name,
    string Description,
    string PreRequisites
);
