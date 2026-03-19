using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.LabTest.Contracts;

public record LabTestResponse(
    Guid Id,
    string Name,
    string Description,
    string PreRequisites,
    decimal Price,
    bool IsAvailableAtHome
);
