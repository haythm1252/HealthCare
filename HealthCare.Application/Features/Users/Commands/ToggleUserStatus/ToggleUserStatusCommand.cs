using HealthCare.Application.Common.Result;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.Users.Commands.BlockUser;

public record ToggleUserStatusCommand(string UserId) : IRequest<Result>;
