using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.LabTest.Contracts;

public record AddLabTestRequest(
    Guid TestId,
    decimal Price,
    bool IsAvailableAtHome
);
