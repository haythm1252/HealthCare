using HealthCare.Application.Common.Result;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.LabAppointment.Commands.AddTestResult;

public record AddTestResultCommand(
    string UserId,
    Guid AppointmentId,
    Guid TestResultId,
    decimal? Value,
    string? Summary,
    IFormFile? ResultFile
) : IRequest<Result>;