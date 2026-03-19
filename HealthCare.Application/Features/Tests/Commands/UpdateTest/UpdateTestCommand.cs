using HealthCare.Application.Common.Result;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.Tests.Commands.UpdateTest;

public record UpdateTestCommand(
    Guid Id,
    string Name,
    string Description,
    string PreRequisites
) : IRequest<Result>;
