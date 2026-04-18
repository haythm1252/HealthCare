using FluentValidation;
using HealthCare.Application.Common.Result;
using HealthCare.Application.Errors;
using HealthCare.Application.Interfaces.Repositories.UnitOfWork;
using HealthCare.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace HealthCare.Application.Features.Appointments.Commands.AppointmentConfirmation;

public class AppointmentConfrimationCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<AppointmentConfrimationCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result> Handle(AppointmentConfrimationCommand request, CancellationToken cancellationToken)
    {
        var appointmentType = Enum.Parse<TargetType>(request.AppointmentType, true);
        var status = Enum.Parse<AppointmentStatus>(request.Status, true);

        return appointmentType switch
        {
            TargetType.Doctor => await HandleDoctorAppointmentStatus(request, status, cancellationToken),
            TargetType.Nurse => await HandleNurseAppointmentStatus(request, status, cancellationToken),
            TargetType.Lab => await HandleLabAppointmentStatus(request, status, cancellationToken),
            _ => Result.Failure(AppointmentErrors.UnSupportedType)
        };
    }

    private async Task<Result> HandleDoctorAppointmentStatus(AppointmentConfrimationCommand request, AppointmentStatus status, CancellationToken cancellationToken)
    {
        var appointment = await _unitOfWork.DoctorAppointments.AsQueryable()
            .Include(da => da.DoctorSlot)
            .Where(da => da.Id == request.AppointmentId && da.Doctor.UserId == request.UserId)
            .SingleOrDefaultAsync(cancellationToken);

        if (appointment is null) 
            return Result.Failure(AppointmentErrors.NotFound);
        if (appointment.Status != AppointmentStatus.Pending)
            return Result.Failure(AppointmentErrors.InvalidStatus);
        if (appointment.AppointmentType != AppointmentType.HomeVisit)
            return Result.Failure(AppointmentErrors.NotHomeVisit);

        if (appointment.DoctorSlot.Date.ToDateTime(appointment.DoctorSlot.StartTime) < DateTime.UtcNow)
        {
            appointment.Status = AppointmentStatus.Declined;
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Failure(AppointmentErrors.AppointmentExpired);
        }

        appointment.Status = status;
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    private async Task<Result> HandleNurseAppointmentStatus(AppointmentConfrimationCommand request, AppointmentStatus status, CancellationToken cancellationToken)
    {
        var appointment = await _unitOfWork.NurseAppointments.AsQueryable()
            .Include(na => na.NurseShift)
            .Where(na => na.Id == request.AppointmentId && na.Nurse.UserId == request.UserId)
            .SingleOrDefaultAsync(cancellationToken);

        if (appointment is null) 
            return Result.Failure(AppointmentErrors.NotFound);
        if (appointment.Status != AppointmentStatus.Pending) 
            return Result.Failure(AppointmentErrors.InvalidStatus);

        if (appointment.NurseShift.Date.ToDateTime(appointment.StartTime) < DateTime.UtcNow)
        {
            appointment.Status = AppointmentStatus.Declined;
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Failure(AppointmentErrors.AppointmentExpired);
        }

        appointment.Status = status;
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    private async Task<Result> HandleLabAppointmentStatus(AppointmentConfrimationCommand request, AppointmentStatus status, CancellationToken cancellationToken)
    {
        var appointment = await _unitOfWork.LabAppointments.AsQueryable()
            .Include(la => la.TestResults)
            .Where(la => la.Id == request.AppointmentId && la.Lab.UserId == request.UserId)
            .SingleOrDefaultAsync(cancellationToken);

        if (appointment is null) 
            return Result.Failure(AppointmentErrors.NotFound);
        if (appointment.Status != AppointmentStatus.Pending) 
            return Result.Failure(AppointmentErrors.InvalidStatus);
        if (appointment.AppointmentType != AppointmentType.HomeVisit) 
            return Result.Failure(AppointmentErrors.NotHomeVisit);

        if (appointment.Date.ToDateTime(appointment.StartTime) < DateTime.UtcNow)
        {
            appointment.Status = AppointmentStatus.Declined;
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Failure(AppointmentErrors.AppointmentExpired);
        }

        using var transaction = await _unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            appointment.Status = status;
            var res = await _unitOfWork.SaveChangesAsync(cancellationToken);

            if (res <= 0)
            {
                await transaction.RollbackAsync(cancellationToken); 
                return Result.Failure(LabAppointmentErrors.SaveFailed);
            }

            if (status == AppointmentStatus.Confirmed)
            {
                var testIds = appointment.TestResults.Select(tr => tr.TestId).ToList();

                await _unitOfWork.DoctorAppointmentTests.AsQueryable()
                    .Where(rt => rt.DoctorAppointment.PatientId == appointment.PatientId
                        && testIds.Contains(rt.TestId)
                        && rt.Status == TestResultStatus.Pending)
                    .ExecuteUpdateAsync(setters => setters.SetProperty(a => a.Status, TestResultStatus.InProgress), cancellationToken);
            }

            await transaction.CommitAsync(cancellationToken);
            return Result.Success();
        }
        catch (Exception)
        {
            await transaction.RollbackAsync(cancellationToken);
            return Result.Failure(LabAppointmentErrors.SaveFailed);
        }
    }
}