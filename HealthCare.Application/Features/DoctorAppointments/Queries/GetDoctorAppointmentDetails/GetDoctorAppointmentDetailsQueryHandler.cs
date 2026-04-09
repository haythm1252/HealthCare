using HealthCare.Application.Common.Consts;
using HealthCare.Application.Common.Result;
using HealthCare.Application.Errors;
using HealthCare.Application.Features.DoctorAppointments.Contracts;
using HealthCare.Application.Interfaces.Repositories.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.DoctorAppointments.Queries.GetDoctorAppointmentDetails;

public class GetDoctorAppointmentDetailsQueryHandler(IUnitOfWork unitOfWork) 
    : IRequestHandler<GetDoctorAppointmentDetailsQuery, Result<DoctorAppointmentDetailsResponse>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result<DoctorAppointmentDetailsResponse>> Handle(GetDoctorAppointmentDetailsQuery request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.DoctorAppointments.AsQueryable()
            .AsNoTracking()
            .AsSplitQuery()
            .Where(a => a.Id == request.AppointmentId);

        if (request.UserRole == DefaultRoles.Patient)
            query = query.Where(a => a.Patient.UserId == request.UserId);
        else if (request.UserRole == DefaultRoles.Doctor)
            query = query.Where(a => a.Doctor.UserId == request.UserId);
        else 
            return Result.Failure<DoctorAppointmentDetailsResponse>(AppointmentErrors.NotFound);


        var appointment = await query
            .Select(a => new DoctorAppointmentDetailsResponse(
                a.Id,
                a.DoctorId,
                a.PatientId,
                a.Doctor.User.Name,
                a.AppointmentType,
                a.Status,
                a.DoctorSlot.Date,
                a.DoctorSlot.StartTime,
                a.DoctorSlot.EndTime,
                a.Fee,
                a.Notes,
                a.Address,
                a.PaymentType,
                a.Diagnosis,
                a.Prescriptions,

                a.DoctorAppointmentTests.Select(t => new RequiredTestDto(
                    t.TestId,
                    t.Test.Name,
                    t.Status
                ))
            )).SingleOrDefaultAsync(cancellationToken);

        if (appointment is null)
            return Result.Failure<DoctorAppointmentDetailsResponse>(AppointmentErrors.NotFound);

        return Result.Success(appointment);
    }
}
