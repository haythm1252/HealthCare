using HealthCare.Application.Common.Result;
using HealthCare.Application.Errors;
using HealthCare.Application.Features.Labs.Contracts;
using HealthCare.Application.Interfaces.Repositories.UnitOfWork;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.Labs.Queries.GetLabSchedule;

public class LabScheduleQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<LabScheduleQuery, Result<LabScheduleResponse>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result<LabScheduleResponse>> Handle(LabScheduleQuery request, CancellationToken cancellationToken)
    {
        var labSchedule = await _unitOfWork.Labs.AsQueryable()
            .AsNoTracking()
            .Where(l => l.UserId == request.UserId)
            .ProjectToType<LabScheduleResponse>()
            .SingleOrDefaultAsync(cancellationToken);

        return labSchedule is null ?
            Result.Failure<LabScheduleResponse>(UserErrors.NotFound) :
            Result.Success(labSchedule);
    }
}
