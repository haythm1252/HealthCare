using HealthCare.Application.Common.Result;
using HealthCare.Application.Errors;
using HealthCare.Application.Features.Appointments.Contracts;
using HealthCare.Application.Features.DoctorAppointments.Contracts;
using HealthCare.Application.Interfaces.Repositories.UnitOfWork;
using HealthCare.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.DoctorAppointments.Queries.PaymentStatus;

public class DoctorAppointmentPaymentStatusQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<DoctorAppointmentPaymentStatusQuery, Result<AppointmentPaymentStatusResponse>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result<AppointmentPaymentStatusResponse>> Handle(DoctorAppointmentPaymentStatusQuery request, CancellationToken cancellationToken)
    {
        var patientId = await _unitOfWork.Patients.AsQueryable()
            .Where(p => p.UserId == request.UserId)
            .Select(p => p.Id)
            .SingleOrDefaultAsync(cancellationToken);

        if (patientId == Guid.Empty)
            return Result.Failure<AppointmentPaymentStatusResponse>(UserErrors.NotFound);

        var appointment = await _unitOfWork.DoctorAppointments.AsQueryable()
            .AsNoTracking() 
            .Where(a => a.Id == request.AppointmentId && a.PatientId == patientId)
            .Select(a => new AppointmentPaymentStatusResponse
            (
                AppointmentId: a.Id,
                DoctorName: a.Doctor.User.Name,
                DoctorPhoneNumber: a.Doctor.User.PhoneNumber!,
                Date: a.DoctorSlot.Date,
                StartTime: a.DoctorSlot.StartTime,
                EndTime: a.DoctorSlot.EndTime,
                AppointmentType: a.AppointmentType,
                TotalFee: a.Fee,
                Status: a.Status,
                PaymentType: a.PaymentType,
                PaymentStatus: a.PaymentStatus
            ))
            .SingleOrDefaultAsync(cancellationToken);

        if (appointment is null)
            return Result.Failure<AppointmentPaymentStatusResponse>(AppointmentErrors.NotFound);

        if (appointment.AppointmentType is not AppointmentType.Online)
            return Result.Failure<AppointmentPaymentStatusResponse>(AppointmentErrors.PaymentNotSupported);

        return Result.Success(appointment);
    }
}
