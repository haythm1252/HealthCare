using HealthCare.Application.Common.Result;
using MediatR;
using Microsoft.EntityFrameworkCore.Query.Internal;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.LabTest.Commands.DeleteLabTest;

public record DeleteLabTestCommand(
    string UserId,
    Guid LabTestId
) : IRequest<Result>;
