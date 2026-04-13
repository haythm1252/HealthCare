using HealthCare.Application.Common.Result;
using HealthCare.Application.Errors;
using HealthCare.Application.Interfaces.Repositories.UnitOfWork;
using HealthCare.Application.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HealthCare.Application.Features.Community.Commands.DeletePost;

public class DeletePostCommandHandler(IUnitOfWork unitOfWork, ICloudinaryService cloudinaryService) : IRequestHandler<DeletePostCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ICloudinaryService _cloudinaryService = cloudinaryService;

    public async Task<Result> Handle(DeletePostCommand request, CancellationToken cancellationToken)
    {
        var post = await _unitOfWork.Posts.AsQueryable()
            .FirstOrDefaultAsync(p => p.Id == request.PostId && p.Doctor.UserId == request.UserId, cancellationToken);

        if (post is null)
            return Result.Failure(PostErrors.NotFoundOrUnAuthorize);

        // Delete image from cloudinary if exists
        if (post.IsContainsMedia)
        {
            var deleteImageResult = await _cloudinaryService.DeleteImageAsync(post.AttachmentPublicId!);
            if (deleteImageResult.IsFailure)
                return Result.Failure(deleteImageResult.Error);
        }

        await _unitOfWork.Posts.Delete(post);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
