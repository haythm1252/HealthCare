using HealthCare.Application.Common.Result;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.Tests.Commands.DeleteTest;

public record DeleteTestCommand(Guid Id) : IRequest<Result>;
