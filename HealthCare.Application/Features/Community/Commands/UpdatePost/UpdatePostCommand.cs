using HealthCare.Application.Common.Result;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace HealthCare.Application.Features.Community.Commands.UpdatePost;

public record UpdatePostCommand(
    string UserId,
    Guid PostId,
    Guid SpecialtyId,
    string Title,
    string Content,
    IFormFile? AttachmentFile = null
) : IRequest<Result>;
