using HealthCare.Application.Common.Pagination;
using HealthCare.Application.Features.Labs.Contracts;
using HealthCare.Application.Interfaces.Repositories.UnitOfWork;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.Labs.Queries.GetLabs;

public class GetLabsQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetLabsQuery, PagedList<LabResponse>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<PagedList<LabResponse>> Handle(GetLabsQuery request, CancellationToken cancellationToken)
    {
        return await _unitOfWork.Labs.GetLabsWithFiltersAsync(request, cancellationToken);  
    }
}
