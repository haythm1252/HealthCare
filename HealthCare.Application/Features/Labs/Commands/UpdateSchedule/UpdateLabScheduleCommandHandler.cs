using HealthCare.Application.Common.Result;
using HealthCare.Application.Errors;
using HealthCare.Application.Interfaces.Repositories.UnitOfWork;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.Labs.Commands.UpdateSchedule;

public class UpdateLabScheduleCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<UpdateLabScheduleCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result> Handle(UpdateLabScheduleCommand request, CancellationToken cancellationToken)
    {
        var lab = await _unitOfWork.Labs
            .AsQueryable()
            .SingleOrDefaultAsync(l => l.UserId == request.UserId, cancellationToken);

        if(lab is null)
            return Result.Failure(UserErrors.NotFound);

        request.Adapt(lab);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
