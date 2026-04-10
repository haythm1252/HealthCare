using HealthCare.Application.Common.Result;
using HealthCare.Application.Errors;
using HealthCare.Application.Features.Specialities.Contracts;
using HealthCare.Application.Interfaces.Repositories.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.Specialities.Commands.UpdateSpeciality;

public class UpdateSpecialityCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<UpdateSpecialityCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result> Handle(UpdateSpecialityCommand request, CancellationToken cancellationToken)
    {
        var specilty = await _unitOfWork.Specialties.AsQueryable()
            .Where(s => s.Id == request.Id)
            .SingleOrDefaultAsync(cancellationToken);

        if(specilty is null)
            return Result.Failure(SpecialtyErrors.NotFound);

        var isNameExist = await _unitOfWork.Specialties.AnyAsync(s => s.Name.ToLower() == request.Name.ToLower(), cancellationToken);
        if (isNameExist)
            return Result.Failure(SpecialtyErrors.AlreadyExists);

        specilty.Name = request.Name;
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
