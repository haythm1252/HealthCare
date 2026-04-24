using HealthCare.Application.Common.Result;
using HealthCare.Application.Features.Communication.Contracts;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.Communication.Commands.GetOrGenerateMeeting;

public record GetOrGenerateMeetingCommand(
    string UserId,
    Guid AppointmentId
) : IRequest<Result<MeetingResponse>>;
