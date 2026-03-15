using HealthCare.Application.Common.Pagination;
using HealthCare.Application.Features.Nurses.Contracts;
using HealthCare.Application.Interfaces.Repositories.UnitOfWork;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.Nurses.Queries.GetNurses;

public class GetNursesQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetNursesQuery, PagedList<NurseResponse>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<PagedList<NurseResponse>> Handle(GetNursesQuery request, CancellationToken cancellationToken)
    {
        return await _unitOfWork.Nurses.GetNursesWithFiltersAsync(request, cancellationToken);  
    }
}
