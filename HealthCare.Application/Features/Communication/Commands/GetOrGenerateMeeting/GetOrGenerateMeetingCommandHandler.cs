using HealthCare.Application.Common.Result;
using HealthCare.Application.Features.Communication.Contracts;
using HealthCare.Application.Interfaces.Repositories.UnitOfWork;
using HealthCare.Application.Services;
using HealthCare.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.Communication.Commands.GetOrGenerateMeeting;

public class GetOrGenerateMeetingCommandHandler(IUnitOfWork unitOfWork, IMeetingService meetingService) 
    : IRequestHandler<GetOrGenerateMeetingCommand, Result<MeetingResponse>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IMeetingService _meetingService = meetingService;

    public async Task<Result<MeetingResponse>> Handle(GetOrGenerateMeetingCommand request, CancellationToken cancellationToken)
    {
        var now = TimeOnly.FromDateTime(DateTime.Now);
        var today = DateOnly.FromDateTime(DateTime.Now);

        var appointment = await _unitOfWork.DoctorAppointments.AsQueryable()
            .Where(a => a.Id == request.AppointmentId &&
                (a.Doctor.UserId == request.UserId || a.Patient.UserId == request.UserId))
            .Include(a => a.DoctorSlot)
            .FirstOrDefaultAsync(cancellationToken);

        if (appointment is null)
            return Result.Failure<MeetingResponse>(new Error("DoctorAppointment.NotExist", 
                "Appointment not Exist or u dont have permision", 404));

        if (appointment.AppointmentType is not AppointmentType.Online || appointment.PaymentStatus is not PaymentStatus.Paid)
            return Result.Failure<MeetingResponse>(new Error("DoctorAppointment.InvalidStatus", 
                "Appointment is not online or not paid", 400));

        if (appointment.DoctorSlot.Date != today)
            return Result.Failure<MeetingResponse>(new Error("DoctorAppointment.InvalidDate", 
                $"Appointment not scheduled for today, appointment date is {appointment.DoctorSlot.Date}", 400));

        if (appointment.DoctorSlot.StartTime > now)
            return Result.Failure<MeetingResponse>(new Error("DoctorAppointment.InvalidStartTime", 
                $"Appointment not started yet, appointment start time is {appointment.DoctorSlot.StartTime}", 400));

        // could give error if the appointment end at the mid night
        if (appointment.DoctorSlot.EndTime.AddHours(1) < now)
            return Result.Failure<MeetingResponse>(new Error("DoctorAppointment.MeetingEnd", 
                $"Appointment has ended, appointment end time was {appointment.DoctorSlot.EndTime}", 400));


        // if meeting url already exist return it
        if (!string.IsNullOrEmpty(appointment.MeetingUrl))
            return Result.Success(new MeetingResponse(appointment.MeetingUrl));


        // generate meeting url if not already exist and save it to database
        var endTime = appointment.DoctorSlot.Date.ToDateTime(appointment.DoctorSlot.EndTime);

        var meetingResult = await _meetingService.CreateMeetingAsync(appointment.Id, endTime);

        if (meetingResult.IsFailure)
            return Result.Failure<MeetingResponse>(meetingResult.Error);

        appointment.MeetingUrl = meetingResult.Value;
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(new MeetingResponse(meetingResult.Value));
    }
}
