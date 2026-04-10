using HealthCare.Application.Common.Result;
using HealthCare.Application.Errors;
using HealthCare.Application.Features.LabTest.Contracts;
using HealthCare.Application.Interfaces.Repositories.UnitOfWork;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.LabTest.Queries.GetLabTests;

public class GetLabTestsQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetLabTestsQuery, Result<IEnumerable<LabTestResponse>>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result<IEnumerable<LabTestResponse>>> Handle(GetLabTestsQuery request, CancellationToken cancellationToken)
    {
        var labExists = await _unitOfWork.Labs
            .AnyAsync(l => l.UserId == request.UserId, cancellationToken);

        if (!labExists)
            return Result.Failure<IEnumerable<LabTestResponse>>(UserErrors.NotFound);

        var tests = await _unitOfWork.LabTests.AsQueryable()
            .AsNoTracking()
            .Where(lt => lt.Lab.UserId == request.UserId && !lt.IsDeleted)
            .OrderByDescending(lt => lt.CreatedAt)
            .ProjectToType<LabTestResponse>()
            .ToListAsync(cancellationToken);

        return Result.Success<IEnumerable<LabTestResponse>>(tests);
    }
}
