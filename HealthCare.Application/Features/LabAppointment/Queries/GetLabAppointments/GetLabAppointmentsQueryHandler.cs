using HealthCare.Application.Common.Pagination;
using HealthCare.Application.Common.Result;
using HealthCare.Application.Errors;
using HealthCare.Application.Features.LabAppointment.Contracts;
using HealthCare.Application.Interfaces.Repositories.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.LabAppointment.Queries.GetLabAppointments;

public class GetLabAppointmentsQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetLabAppointmentsQuery, Result<PagedList<LabAppointmentResponse>>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result<PagedList<LabAppointmentResponse>>> Handle(GetLabAppointmentsQuery request, CancellationToken cancellationToken)
    {
        var labId = await _unitOfWork.Labs.AsQueryable()
            .Where(l => l.UserId == request.UserId)
            .Select(l => l.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (labId == Guid.Empty)
            return Result.Failure<PagedList<LabAppointmentResponse>>(UserErrors.NotFound);


        var appointments = await _unitOfWork.LabAppointments
            .GetLabAppointmentsWithFiltersAsync(labId, request, cancellationToken);

        return Result.Success(appointments);
    }
}