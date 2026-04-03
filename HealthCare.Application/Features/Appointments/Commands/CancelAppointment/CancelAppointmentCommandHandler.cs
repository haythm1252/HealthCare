using HealthCare.Application.Common.Result;
using HealthCare.Application.Errors;
using HealthCare.Application.Interfaces.Repositories.UnitOfWork;
using HealthCare.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.Appointments.Commands.CancelAppointment;

public class CancelAppointmentCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<CancelAppointmentCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result> Handle(CancelAppointmentCommand request, CancellationToken cancellationToken)
    {
        var appointmentType = Enum.Parse<TargetType>(request.AppointmentType, true);

        var patientId = await _unitOfWork.Patients.AsQueryable()
            .Where(p => p.UserId == request.UserId)
            .Select(p => p.Id)
            .SingleOrDefaultAsync(cancellationToken);

        if(patientId == Guid.Empty)
            return Result.Failure(UserErrors.NotFound);

        switch (appointmentType)
        {
            case TargetType.Doctor:
                var doctorAppointment = await _unitOfWork.DoctorAppointments.AsQueryable()
                    .Include(da => da.DoctorSlot)
                    .Where(da => da.Id == request.AppointmentId && da.PatientId == patientId)
                    .SingleOrDefaultAsync(cancellationToken);

                if(doctorAppointment is null)
                    return Result.Failure(AppointmentErrors.NotFound);

                if(doctorAppointment.Status == AppointmentStatus.Cancelled)
                    return Result.Failure(AppointmentErrors.AlreadyCancelled);

                if(doctorAppointment.AppointmentType == AppointmentType.Online)
                    return Result.Failure(AppointmentErrors.OnlineCancellationForbidden);

                var doctorCancellationDeadline = doctorAppointment.DoctorSlot.Date.ToDateTime(doctorAppointment.DoctorSlot.StartTime).AddHours(-4);

                if (DateTime.UtcNow > doctorCancellationDeadline)
                    return Result.Failure(AppointmentErrors.CancellationNotAllowed);

                doctorAppointment.Status = AppointmentStatus.Cancelled;
                doctorAppointment.DoctorSlot.IsBooked = false;
                break;

            case TargetType.Nurse:
                var nurseAppointment = await _unitOfWork.NurseAppointments.AsQueryable()
                    .Include(da => da.NurseShift)
                    .Where(da => da.Id == request.AppointmentId && da.PatientId == patientId)
                    .SingleOrDefaultAsync(cancellationToken);

                if (nurseAppointment is null)
                    return Result.Failure(AppointmentErrors.NotFound);

                var nurseCancellationDeadline = nurseAppointment.NurseShift.Date.ToDateTime(nurseAppointment.StartTime);

                if (DateTime.UtcNow > nurseCancellationDeadline)
                    return Result.Failure(AppointmentErrors.CancellationNotAllowed);

                if (nurseAppointment.Status == AppointmentStatus.Cancelled)
                    return Result.Failure(AppointmentErrors.AlreadyCancelled);

                nurseAppointment.Status = AppointmentStatus.Cancelled;
                break;

            case TargetType.Lab:
                var labAppointment = await _unitOfWork.LabAppointments.AsQueryable()
                    .Where(da => da.Id == request.AppointmentId && da.PatientId == patientId)
                    .SingleOrDefaultAsync(cancellationToken);

                if (labAppointment is null)
                    return Result.Failure(AppointmentErrors.NotFound);

                if (DateTime.UtcNow > labAppointment.Date.ToDateTime(labAppointment.StartTime)
                    || labAppointment.Status == AppointmentStatus.Completed || labAppointment.Status == AppointmentStatus.NoShow)
                    return Result.Failure(AppointmentErrors.CancellationNotAllowed);

                if (labAppointment.Status == AppointmentStatus.Cancelled)
                    return Result.Failure(AppointmentErrors.AlreadyCancelled);

                labAppointment.Status = AppointmentStatus.Cancelled;
                break;
            default:
                return Result.Failure(AppointmentErrors.UnSupportedType);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
