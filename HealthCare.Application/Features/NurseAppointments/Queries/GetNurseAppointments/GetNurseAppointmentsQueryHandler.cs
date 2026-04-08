using HealthCare.Application.Common.Pagination;
using HealthCare.Application.Common.Result;
using HealthCare.Application.Errors;
using HealthCare.Application.Features.NurseAppointments.Contracts;
using HealthCare.Application.Interfaces.Repositories.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.NurseAppointments.Queries.GetNurseAppointments;

public class GetNurseAppointmentsQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetNurseAppointmentsQuery, Result<PagedList<NurseAppointmentResponse>>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result<PagedList<NurseAppointmentResponse>>> Handle(GetNurseAppointmentsQuery request, CancellationToken cancellationToken)
    {
        var nurseId = await _unitOfWork.Nurses.AsQueryable()
            .Where(n => n.UserId == request.UserId)
            .Select(n => n.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (nurseId == Guid.Empty)
            return Result.Failure<PagedList<NurseAppointmentResponse>>(UserErrors.NotFound);

        var appointments = await _unitOfWork.NurseAppointments
            .GetNurseAppointmentsWithFiltersAsync(nurseId, request, cancellationToken);

        return Result.Success(appointments);
    }
}
