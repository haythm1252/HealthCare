using HealthCare.Application.Common.Result;
using HealthCare.Application.Errors;
using HealthCare.Application.Features.Appointments.Commands.AppointmentConfirmation;
using HealthCare.Application.Interfaces.Repositories.UnitOfWork;
using HealthCare.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.Appointments.Commands.UpdateFinalStatus;

public class UpdateAppointmentFinalStatusCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<UpdateAppointmentFinalStatusCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result> Handle(UpdateAppointmentFinalStatusCommand request, CancellationToken cancellationToken)
    {
        var appointmentType = Enum.Parse<TargetType>(request.AppointmentType, true);
        var status = Enum.Parse<AppointmentStatus>(request.Status, true);

        return appointmentType switch
        {
            TargetType.Doctor => await HandleDoctorAppointmentFinalStatus(request, status, cancellationToken),
            TargetType.Nurse => await HandleNurseAppointmentFinalStatus(request, status, cancellationToken),
            TargetType.Lab => await HandleLabAppointmentFinalStatus(request, status, cancellationToken),
            _ => Result.Failure(AppointmentErrors.UnSupportedType)
        };
    }

    private async Task<Result> HandleDoctorAppointmentFinalStatus(UpdateAppointmentFinalStatusCommand request, AppointmentStatus status, CancellationToken cancellationToken)
    {
        var appointment = await _unitOfWork.DoctorAppointments.AsQueryable()
            .Include(da => da.DoctorSlot)
            .Where(da => da.Id == request.AppointmentId && da.Doctor.UserId == request.UserId)
            .SingleOrDefaultAsync(cancellationToken);

        if (appointment is null)
            return Result.Failure(AppointmentErrors.NotFound);
        
        if (appointment.Status == AppointmentStatus.Completed || appointment.Status == AppointmentStatus.NoShow)
            return Result.Failure(new Error("Appointment.AlreadyUpadated", $"The appointment already has a final status {appointment.Status}",409));
        
        if (appointment.Status != AppointmentStatus.Confirmed)
            return Result.Failure(AppointmentErrors.NotConfirmed);



        if (appointment.DoctorSlot.Date.ToDateTime(appointment.DoctorSlot.StartTime) > DateTime.UtcNow)
            return Result.Failure(AppointmentErrors.TooEarlyToFinalize);


        appointment.Status = status;
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    private async Task<Result> HandleNurseAppointmentFinalStatus(UpdateAppointmentFinalStatusCommand request, AppointmentStatus status, CancellationToken cancellationToken)
    {
        var appointment = await _unitOfWork.NurseAppointments.AsQueryable()
            .Include(na => na.NurseShift)
            .Where(na => na.Id == request.AppointmentId && na.Nurse.UserId == request.UserId)
            .SingleOrDefaultAsync(cancellationToken);

        if (appointment is null)
            return Result.Failure(AppointmentErrors.NotFound);
        
        if (appointment.Status == AppointmentStatus.Completed || appointment.Status == AppointmentStatus.NoShow)
            return Result.Failure(new Error("Appointment.AlreadyUpadated", $"The appointment already has a final status {appointment.Status}", 409));
       
        if (appointment.Status != AppointmentStatus.Confirmed)
            return Result.Failure(AppointmentErrors.NotConfirmed);

        if (appointment.NurseShift.Date.ToDateTime(appointment.StartTime) > DateTime.UtcNow)
            return Result.Failure(AppointmentErrors.TooEarlyToFinalize);

        appointment.Status = status;
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    private async Task<Result> HandleLabAppointmentFinalStatus(UpdateAppointmentFinalStatusCommand request, AppointmentStatus status, CancellationToken cancellationToken)
    {
        var appointment = await _unitOfWork.LabAppointments.AsQueryable()
            .Where(la => la.Id == request.AppointmentId && la.Lab.UserId == request.UserId)
            .SingleOrDefaultAsync(cancellationToken);

        if (appointment is null)
            return Result.Failure(AppointmentErrors.NotFound);

        if (appointment.Status == AppointmentStatus.Completed || appointment.Status == AppointmentStatus.NoShow)
            return Result.Failure(new Error("Appointment.AlreadyUpadated", $"The appointment already has a final status {appointment.Status}", 409));

        if (appointment.Status != AppointmentStatus.Confirmed)
            return Result.Failure(AppointmentErrors.NotConfirmed);


        if (appointment.Date.ToDateTime(appointment.StartTime) > DateTime.UtcNow)
            return Result.Failure(AppointmentErrors.TooEarlyToFinalize);

        appointment.Status = status;
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
