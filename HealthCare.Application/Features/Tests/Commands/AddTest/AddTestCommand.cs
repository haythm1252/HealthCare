using HealthCare.Application.Common.Result;
using HealthCare.Application.Features.Tests.Contracts;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.Tests.Commands.AddTest;

public record AddTestCommand(
    string Name,
    string Description,
    string PreRequisites
) : IRequest<TestResponse>;
