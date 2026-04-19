using HealthCare.Application.Common.Result;
using HealthCare.Application.Interfaces.Repositories.UnitOfWork;
using HealthCare.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace HealthCare.Application.Features.Communication.Commands.JoinVideo;

public class JoinVideoCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<JoinVideoCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result> Handle(JoinVideoCommand request, CancellationToken cancellationToken)
    {
        var now = TimeOnly.FromDateTime(DateTime.Now);
        var today = DateOnly.FromDateTime(DateTime.Now);

        var appointment = await _unitOfWork.DoctorAppointments.AsQueryable()
            .Where(a => a.Id == request.AppointmentId && 
                (a.Doctor.UserId == request.UserId || a.Patient.UserId == request.UserId))
            .Select(a => new
            {
                a.Id,
                a.DoctorSlot.StartTime,
                a.DoctorSlot.EndTime,
                a.DoctorSlot.Date,
                a.AppointmentType,
                a.PaymentStatus
            })
            .FirstOrDefaultAsync(cancellationToken);    

        if(appointment is null)
            return Result.Failure(new Error("DoctorAppointment","Appointment not Exist", 404));


        if(appointment.AppointmentType is not Domain.Enums.AppointmentType.Online || appointment.PaymentStatus is not PaymentStatus.Paid)
            return Result.Failure(new Error("DoctorAppointment","Appointment is not online or not paid", 400));


        if (appointment.Date != today)
            return Result.Failure(new Error("DoctorAppointment","Appointment not scheduled for today", 400));

        if (appointment.StartTime > now)
            return Result.Failure(new Error("DoctorAppointment","Appointment not started yet", 400));

        // could give error if the appointment end at the mid night
        if (appointment.EndTime.AddHours(1) < now)
            return Result.Failure(new Error("DoctorAppointment","Appointment has ended", 400));


        return Result.Success();
    }
}
