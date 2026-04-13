using HealthCare.Application.Common.Result;
using MediatR;

namespace HealthCare.Application.Features.Community.Commands.DeletePost;

public record DeletePostCommand(
    Guid PostId,
    string UserId
) : IRequest<Result>;
