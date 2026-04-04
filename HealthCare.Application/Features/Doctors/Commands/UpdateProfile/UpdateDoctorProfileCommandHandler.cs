using HealthCare.Application.Common.Result;
using HealthCare.Application.Errors;
using HealthCare.Application.Interfaces.Repositories.UnitOfWork;
using HealthCare.Application.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.Doctors.Commands.UpdateProfile;

public class UpdateDoctorProfileCommandHandler(IUnitOfWork unitOfWork, ICloudinaryService fileService)
    : IRequestHandler<UpdateDoctorProfileCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ICloudinaryService _cloudinaryService = fileService;

    public async Task<Result> Handle(UpdateDoctorProfileCommand request, CancellationToken cancellationToken)
    {
        var doctor = await _unitOfWork.Doctors.AsQueryable()
                .Where(n => n.UserId == request.UserId)
                .Include(n => n.User)
                .SingleOrDefaultAsync(cancellationToken);
        
        if (doctor is null)
            return Result.Failure(UserErrors.NotFound);

        if (request.ProfilePicture is not null)
        {
            using var stream = request.ProfilePicture.OpenReadStream();

            var result = await _cloudinaryService.UploadImageAsync(stream, request.ProfilePicture.FileName);
            if (result.IsFailure)
                return Result.Failure(result.Error);

            if (!string.IsNullOrWhiteSpace(doctor.ProfilePicturePublicId))
            {
                var deleteingResult = await _cloudinaryService.DeleteImageAsync(doctor.ProfilePicturePublicId);
                if (deleteingResult.IsFailure)
                    return Result.Failure(deleteingResult.Error);
            }

            doctor.ProfilePictureUrl = result.Value.Url;
            doctor.ProfilePicturePublicId = result.Value.PublicId;
        }

        doctor.User.Name = request.Name;
        doctor.User.PhoneNumber = request.PhoneNumber;
        doctor.User.Address = request.Address;
        doctor.User.AddressUrl = request.AddressUrl;
        doctor.User.City = request.City;
        doctor.Bio = request.Bio;
        doctor.Title = request.Title;
        doctor.LastModified = DateTime.UtcNow;

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
