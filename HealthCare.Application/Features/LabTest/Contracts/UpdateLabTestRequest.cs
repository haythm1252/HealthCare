using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.LabTest.Contracts;

public record UpdateLabTestRequest(
    decimal Price,
    bool IsAvailableAtHome
);
