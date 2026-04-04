using HealthCare.Application.Common.Result;
using HealthCare.Application.Errors;
using HealthCare.Application.Interfaces.Repositories.UnitOfWork;
using HealthCare.Application.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.Labs.Commands.UpdateProfile;

public record UpdateLabProfileCommandHandler(IUnitOfWork unitOfWork, ICloudinaryService fileService) : IRequestHandler<UpdateLabProfileCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ICloudinaryService _cloudinaryService = fileService;
    public async Task<Result> Handle(UpdateLabProfileCommand request, CancellationToken cancellationToken)
    {
        var lab = await _unitOfWork.Labs.AsQueryable()
                .Where(n => n.UserId == request.UserId)
                .Include(n => n.User)
                .SingleOrDefaultAsync(cancellationToken);

        if (lab is null)
            return Result.Failure(UserErrors.NotFound);

        if (request.ProfilePicture is not null)
        {
            using var stream = request.ProfilePicture.OpenReadStream();

            var result = await _cloudinaryService.UploadImageAsync(stream, request.ProfilePicture.FileName);
            if (result.IsFailure)
                return Result.Failure(result.Error);

            if (!string.IsNullOrWhiteSpace(lab.ProfilePicturePublicId))
            {
                var deleteingResult = await _cloudinaryService.DeleteImageAsync(lab.ProfilePicturePublicId);
                if (deleteingResult.IsFailure)
                    return deleteingResult;
            }

            lab.ProfilePictureUrl = result.Value.Url;
            lab.ProfilePicturePublicId = result.Value.PublicId;
        }

        lab.User.Name = request.Name;
        lab.User.PhoneNumber = request.PhoneNumber;
        lab.User.Address = request.Address;
        lab.User.AddressUrl = request.AddressUrl;
        lab.User.City = request.City;
        lab.Bio = request.Bio;
        lab .LastModified = DateTime.UtcNow;

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
