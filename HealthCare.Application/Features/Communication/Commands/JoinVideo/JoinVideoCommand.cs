using HealthCare.Application.Common.Result;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.Communication.Commands.JoinVideo;

public record JoinVideoCommand(string UserId, Guid AppointmentId) : IRequest<Result>;
