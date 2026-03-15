using HealthCare.Application.Common.Pagination;
using HealthCare.Application.Features.Doctors.Contracts;
using HealthCare.Application.Interfaces.Repositories.UnitOfWork;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.Doctors.Queries.GetDoctors;

public class GetDoctorsQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetDoctorsQuery, PagedList<DoctorResponse>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<PagedList<DoctorResponse>> Handle(GetDoctorsQuery request, CancellationToken cancellationToken)
    {
        return await _unitOfWork.Doctors.GetDoctorsWithFiltersAsync(request, cancellationToken);
    }
}
