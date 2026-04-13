using HealthCare.Application.Common.Consts;
using HealthCare.Application.Common.Result;
using HealthCare.Application.Errors;
using HealthCare.Application.Interfaces.Repositories.UnitOfWork;
using HealthCare.Application.Services;
using HealthCare.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HealthCare.Application.Features.Community.Commands.AddPost;

public class AddPostCommandHandler(IUnitOfWork unitOfWork, ICloudinaryService cloudinaryService) : IRequestHandler<AddPostCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ICloudinaryService _cloudinaryService = cloudinaryService;

    public async Task<Result> Handle(AddPostCommand request, CancellationToken cancellationToken)
    {
        var doctor = await _unitOfWork.Doctors.AsQueryable()
            .Where(d => d.UserId == request.UserId)
            .Select(d => new {d.Id})
            .FirstOrDefaultAsync(cancellationToken);

        if (doctor is null)
            return Result.Failure(DoctorErrors.NotFound);

        var specialtyExists = await _unitOfWork.Specialties
            .AsQueryable()
            .AnyAsync(s => s.Id == request.SpecialtyId, cancellationToken);

        if (!specialtyExists)
            return Result.Failure(SpecialtyErrors.NotFound);

        string? attachmentUrl = null;
        string? attachmentPublicId = null;
        bool isContainsMedia = false;

        // Upload image if provided
        if (request.AttachmentFile is not null)
        {
            using var stream = request.AttachmentFile.OpenReadStream();
            var uploadResult = await _cloudinaryService.UploadImageAsync(stream, request.AttachmentFile.FileName);

            if (uploadResult.IsFailure)
                return Result.Failure(PostErrors.UploadFailed);

            attachmentUrl = uploadResult.Value.Url;
            attachmentPublicId = uploadResult.Value.PublicId;
            isContainsMedia = true;
        }

        var post = new Post
        {           
            DoctorId = doctor.Id,
            SpecialtyId = request.SpecialtyId,
            Title = request.Title,
            Content = request.Content,
            AttachmentUrl = attachmentUrl,
            AttachmentPublicId = attachmentPublicId,
            IsPublished = false,
            IsContainsMedia = isContainsMedia,
        };

        await _unitOfWork.Posts.AddAsync(post, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
