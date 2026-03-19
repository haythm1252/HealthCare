using HealthCare.Application.Common.Result;
using HealthCare.Application.Features.Labs.Contracts;
using HealthCare.Application.Features.LabTest.Contracts;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.LabTest.Commands.AddLabTest;

public record AddLabTestCommand(
    string UserId,
    Guid TestId,
    decimal Price,
    bool IsAvailableAtHome
) : IRequest<Result<LabTestResponse>>;
