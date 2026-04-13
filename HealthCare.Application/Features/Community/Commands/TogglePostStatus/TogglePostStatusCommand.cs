using HealthCare.Application.Common.Result;
using MediatR;

namespace HealthCare.Application.Features.Community.Commands.TogglePostStatus;

public record TogglePostStatusCommand(
    Guid PostId
) : IRequest<Result>;
