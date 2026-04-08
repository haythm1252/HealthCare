using HealthCare.Application.Common.Pagination;
using HealthCare.Application.Common.Result;
using HealthCare.Application.Errors;
using HealthCare.Application.Features.DoctorAppointments.Contracts;
using HealthCare.Application.Interfaces.Repositories.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.DoctorAppointments.Queries.GetDoctorAppointments;

public class GetDoctorAppointmentsQueryHandler(IUnitOfWork unitOfWork) 
    : IRequestHandler<GetDoctorAppointmentsQuery, Result<PagedList<DoctorAppointmentResponse>>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result<PagedList<DoctorAppointmentResponse>>> Handle(GetDoctorAppointmentsQuery request, CancellationToken cancellationToken)
    {
        var doctorId = await _unitOfWork.Doctors.AsQueryable()
            .Where(d => d.UserId == request.UserId)
            .Select(d => d.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (doctorId == Guid.Empty)
            return Result.Failure<PagedList<DoctorAppointmentResponse>>(UserErrors.NotFound);

        var appointments =  await _unitOfWork.DoctorAppointments
            .GetDoctorAppointmentsWithFiltersAsync(doctorId, request, cancellationToken);

        return Result.Success(appointments);
    }
}
