using HealthCare.Application.Common.Result;
using HealthCare.Application.Errors;
using HealthCare.Application.Interfaces.Repositories.UnitOfWork;
using HealthCare.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.Specialities.Commands.DeleteSpeciality;

public class DeleteSpecialityCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<DeleteSpecialityCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result> Handle(DeleteSpecialityCommand request, CancellationToken cancellationToken)
    {
        var res = await _unitOfWork.Specialties.AsQueryable()
            .Where(s => s.Id == request.Id)
            .ExecuteUpdateAsync(setters => setters.SetProperty(a => a.IsDeleted, true), cancellationToken);

        return res > 0 ? Result.Success() : Result.Failure(SpecialtyErrors.NotFound);
    }
}
