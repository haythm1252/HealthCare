using HealthCare.Application.Common.Result;
using HealthCare.Application.Errors;
using HealthCare.Application.Features.Specialities.Contracts;
using HealthCare.Application.Features.Tests.Contracts;
using HealthCare.Application.Interfaces.Repositories.UnitOfWork;
using HealthCare.Domain.Entities;
using Mapster;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.Specialities.Commands.AddSpeciality;

public class AddSpecialityCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<AddSpecialityCommand, Result<SpecialityResponse>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result<SpecialityResponse>> Handle(AddSpecialityCommand request, CancellationToken cancellationToken)
    {
        var isExist = await _unitOfWork.Specialties.AnyAsync(s => s.Name.ToLower() == request.Name.ToLower() && !s.IsDeleted, cancellationToken);
        if (isExist)
            return Result.Failure<SpecialityResponse>(SpecialtyErrors.AlreadyExists);

        var speciality = new Specialty
        {
            Name = request.Name
        };

        await _unitOfWork.Specialties.AddAsync(speciality, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(speciality.Adapt<SpecialityResponse>());
    }
}
