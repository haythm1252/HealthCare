using HealthCare.Application.Common.Result;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace HealthCare.Application.Features.Community.Commands.AddPost;

public record AddPostCommand(
    string UserId,
    Guid SpecialtyId,
    string Title,
    string Content,
    IFormFile? AttachmentFile = null
) : IRequest<Result>;
