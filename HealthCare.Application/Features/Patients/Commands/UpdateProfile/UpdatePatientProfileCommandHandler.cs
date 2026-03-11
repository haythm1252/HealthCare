using HealthCare.Application.Common.Result;
using HealthCare.Application.Errors;
using HealthCare.Application.Interfaces.Repositories.UnitOfWork;
using HealthCare.Domain.Users;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.Patients.Commands.UpdateProfile;

public class UpdatePatientProfileCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<UpdatePatientProfileCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result> Handle(UpdatePatientProfileCommand request, CancellationToken cancellationToken)
    {
        var patient = await _unitOfWork.Patients
            .AsQueryable()
            .Where(p => p.UserId == request.UserId)
            .Include(p => p.User)
            .SingleOrDefaultAsync(cancellationToken);

        if (patient is null)
            return Result.Failure(UserErrors.NotFound);

        patient.User.Name = request.Name;
        patient.User.Address = request.Address;
        patient.User.AddressUrl = request.AddressUrl;
        patient.User.PhoneNumber = request.PhoneNumber;
        patient.User.City = request.City;
        patient.Weight = request.Weight;
        patient.LastModified = DateTime.UtcNow;

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
