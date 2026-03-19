using HealthCare.Application.Common.Result;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.LabTest.Commands.UpdateLabTest;

public record UpdateLabTestCommand(
    string UserId,
    Guid LabTestId,
    decimal Price,
    bool IsAvailableAtHome
) : IRequest<Result>;
