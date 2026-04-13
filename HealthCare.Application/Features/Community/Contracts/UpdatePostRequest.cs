using Microsoft.AspNetCore.Http;

namespace HealthCare.Application.Features.Community.Contracts;

public record UpdatePostRequest(
    Guid SpecialtyId,
    string Title,
    string Content,
    IFormFile? AttachmentFile = null
);
