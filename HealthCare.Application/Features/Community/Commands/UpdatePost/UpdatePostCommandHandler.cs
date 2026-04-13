using HealthCare.Application.Common.Consts;
using HealthCare.Application.Common.Result;
using HealthCare.Application.Errors;
using HealthCare.Application.Interfaces.Repositories.UnitOfWork;
using HealthCare.Application.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HealthCare.Application.Features.Community.Commands.UpdatePost;

public class UpdatePostCommandHandler(IUnitOfWork unitOfWork, ICloudinaryService cloudinaryService) : IRequestHandler<UpdatePostCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ICloudinaryService _cloudinaryService = cloudinaryService;

    public async Task<Result> Handle(UpdatePostCommand request, CancellationToken cancellationToken)
    {
        var post = await _unitOfWork.Posts.AsQueryable()
            .FirstOrDefaultAsync(p => p.Id == request.PostId && p.Doctor.UserId == request.UserId, cancellationToken);

        if (post is null)
            return Result.Failure(PostErrors.NotFoundOrUnAuthorize);

        var specialtyExists = await _unitOfWork.Specialties.AsQueryable()
            .AnyAsync(s => s.Id == request.SpecialtyId, cancellationToken);

        if (!specialtyExists)
            return Result.Failure(SpecialtyErrors.NotFound);

        // if image exist in the request
        if (request.AttachmentFile is not null)
        {
            using var stream = request.AttachmentFile.OpenReadStream();
            var uploadResult = await _cloudinaryService.UploadImageAsync(stream, request.AttachmentFile.FileName);

            if (uploadResult.IsFailure)
                return Result.Failure(PostErrors.UploadFailed);

            // delete old image if exists
            if (!string.IsNullOrWhiteSpace(post.AttachmentPublicId))
            {
                var deleteResult = await _cloudinaryService.DeleteImageAsync(post.AttachmentPublicId);
                if (deleteResult.IsFailure)
                    return Result.Failure(deleteResult.Error);
            }

            post.AttachmentUrl = uploadResult.Value.Url;
            post.AttachmentPublicId = uploadResult.Value.PublicId;
            post.IsContainsMedia = true;
        }

        post.Title = request.Title;
        post.Content = request.Content;
        post.SpecialtyId = request.SpecialtyId;
        post.LastModified = DateTime.UtcNow;

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
