using HealthCare.Application.Common.Consts;
using HealthCare.Application.Common.Result;
using HealthCare.Application.Errors;
using HealthCare.Application.Features.NurseAppointments.Contracts;
using HealthCare.Application.Features.Nurses.Commands.UpdatePricing;
using HealthCare.Application.Interfaces.Repositories.UnitOfWork;
using HealthCare.Application.Services;
using HealthCare.Domain.Entities;
using HealthCare.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.NurseAppointments.Command.BookNurseAppointment;

public class BookNurseAppointmentCommandHandler(IUnitOfWork unitOfWork, INotificationService notificationService)
    : IRequestHandler<BookNurseAppointmentCommand, Result<BookNurseAppointmentResponse>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly INotificationService _notificationService = notificationService;

    public async Task<Result<BookNurseAppointmentResponse>> Handle(BookNurseAppointmentCommand request, CancellationToken cancellationToken)
    {
        // check if something wrong in the request like patient or nurse or the shift not found also get the data that iwill need
        var patient = await _unitOfWork.Patients.AsQueryable()
            .Where(p => p.UserId == request.UserId)
            .Select(p => new
            {
                p.Id,
                p.User.Name
            })
            .SingleOrDefaultAsync(cancellationToken);

        if (patient is null)
            return Result.Failure<BookNurseAppointmentResponse>(UserErrors.NotFound);

        var nurse = await _unitOfWork.Nurses.AsQueryable()
            .Where(n => n.Id == request.NurseId)
            .Select(n => new
            {
                n.User.Name,
                n.User.Email,
                n.User.PhoneNumber,
                n.HourPrice,
                n.HomeVisitFee
            })
            .SingleOrDefaultAsync(cancellationToken);

        if (nurse is null)
            return Result.Failure<BookNurseAppointmentResponse>(NurseErrors.NotFound);

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var shift = await _unitOfWork.NurseShifts.AsQueryable()
            .Where(ns => ns.Id == request.ShiftId && ns.NurseId == request.NurseId && ns.Date >= today)
            .Select(ns => new { ns.Date, ns.StartTime, ns.EndTime})
            .SingleOrDefaultAsync(cancellationToken);

        if (shift is null)
            return Result.Failure<BookNurseAppointmentResponse>(NurseAppointmentErrors.ShiftNotAvailable);

        var isBooked = await _unitOfWork.NurseAppointments
            .AnyAsync(na => na.PatientId == patient.Id && na.NurseId == request.NurseId && na.NurseShiftId == request.ShiftId && na.Status != AppointmentStatus.Cancelled);

        if (isBooked)
            return Result.Failure<BookNurseAppointmentResponse>(NurseAppointmentErrors.DuplicateBooking);


        // check if the hours the patient added is more than the shift hours
        var serviceTypeEnum = Enum.Parse<NurseServiceType>(request.ServiceType, true);
        var shiftHours = (shift.EndTime - shift.StartTime).TotalHours;

        if(serviceTypeEnum != NurseServiceType.QuickVisit && request.Hours > shiftHours)
            return Result.Failure<BookNurseAppointmentResponse>(new Error("NurseAppointment.ExceedsShiftHours",
            $"The requested hours ({request.Hours}) exceed the available shift duration ({shiftHours} hours).", 409));

        //create the NurseAppointment
        decimal totalFee = serviceTypeEnum == NurseServiceType.QuickVisit
            ? nurse.HomeVisitFee
            : (decimal)(request.Hours * nurse.HourPrice)!;

        var appointment = new NurseAppointment
        {
            PatientId = patient.Id,
            NurseId = request.NurseId,
            NurseShiftId = request.ShiftId,

            Notes = request.Notes,
            Address = request.Address,
            Status = AppointmentStatus.Pending,
            ServiceType = serviceTypeEnum,
            Hours = request.Hours,
            TotalFee = totalFee
        };

        // save it in database and see if it saved or not
        await _unitOfWork.NurseAppointments.AddAsync(appointment, cancellationToken);
        var result = await _unitOfWork.SaveChangesAsync(cancellationToken);

        if (result <= 0)
            return Result.Failure<BookNurseAppointmentResponse>(NurseAppointmentErrors.SaveFailed);

        //heere gonna send the notification email to the nurse
        await _notificationService.SendNewAppointmentNotificationAsync(
            nurse.Email!,
            nurse.Name,
            DefaultRoles.Nurse,
            patient.Name,
            shift.Date.ToString(),
            $"Nurse Appointment {appointment.ServiceType}"
        );

        var response = new BookNurseAppointmentResponse(
            appointment.Id,
            nurse.Name,
            nurse.PhoneNumber!,
            shift.Date,
            request.Address,
            appointment.ServiceType,
            appointment.Hours,
            appointment.TotalFee
        );

        return Result.Success(response);
    }
}
