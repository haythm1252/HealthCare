using HealthCare.Application.Features.Specialities.Contracts;
using HealthCare.Application.Interfaces.Repositories.UnitOfWork;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.Specialities.Queries;

public class GetSpecialitiesQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetSpecialitiesQuery, IEnumerable<SpecialityResponse>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<IEnumerable<SpecialityResponse>> Handle(GetSpecialitiesQuery request, CancellationToken cancellationToken)
    {
        return await _unitOfWork.Specialties
            .AsQueryable()
            .AsNoTracking()
            .ProjectToType<SpecialityResponse>()
            .ToListAsync(cancellationToken);
    }
}
