using HealthCare.Application.Common.Result;
using HealthCare.Application.Errors;
using HealthCare.Application.Features.Patients.Contracts;
using HealthCare.Application.Interfaces.Repositories.UnitOfWork;
using HealthCare.Application.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.Nurses.Commands.UpdateProfile;

public class UpdateNurseProfileCommandHandler(IUnitOfWork unitOfWork, IFileService fileService) : IRequestHandler<UpdateNurseProfileCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IFileService _fileService = fileService;

    public async Task<Result> Handle(UpdateNurseProfileCommand request, CancellationToken cancellationToken)
    {
        var nurse = await _unitOfWork.Nurses.AsQueryable()
                .Where(n => n.UserId == request.UserId)
                .Include(n => n.User)
                .SingleOrDefaultAsync(cancellationToken);

        if (nurse is null)
            return Result.Failure(UserErrors.NotFound);

        if(request.ProfilePicture is not null)
        {
            using var stream = request.ProfilePicture.OpenReadStream();

            var result = await _fileService.UploadImageAsync(stream, request.ProfilePicture.FileName);
            if (result.IsFailure)
                return Result.Failure(result.Error);

            if (!string.IsNullOrWhiteSpace(nurse.ProfilePicturePublicId))
            { 
                var deleteingResult = await _fileService.DeleteImageAsync(nurse.ProfilePicturePublicId);
                if (deleteingResult.IsFailure)
                    return Result.Failure(deleteingResult.Error);
            }

            nurse.ProfilePictureUrl = result.Value.Url;
            nurse.ProfilePicturePublicId = result.Value.PublicId;
        }

        nurse.User.Name = request.Name;
        nurse.User.PhoneNumber = request.PhoneNumber;
        nurse.User.Address = request.Address;
        nurse.User.AddressUrl = request.AddressUrl;
        nurse.User.City = request.City;
        nurse.Bio = request.Bio;
        nurse.LastModified = DateTime.UtcNow;

        await _unitOfWork.SaveChangesAsync(cancellationToken);  
        return Result.Success();
    }
}
