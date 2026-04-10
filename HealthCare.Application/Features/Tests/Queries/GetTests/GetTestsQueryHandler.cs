using FluentValidation;
using HealthCare.Application.Features.Tests.Contracts;
using HealthCare.Application.Interfaces.Repositories.UnitOfWork;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.Tests.Queries.GetTests;

public class GetTestsQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetTestsQuery, IEnumerable<TestResponse>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<IEnumerable<TestResponse>> Handle(GetTestsQuery request, CancellationToken cancellationToken)
    {
        return await _unitOfWork.Tests.AsQueryable()
            .AsNoTracking()
            .Where(t => !t.IsDeleted)
            .ProjectToType<TestResponse>()
            .ToListAsync(cancellationToken);
    }
}
