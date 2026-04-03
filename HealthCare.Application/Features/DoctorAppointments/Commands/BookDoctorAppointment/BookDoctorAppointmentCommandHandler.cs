using HealthCare.Application.Common.Consts;
using HealthCare.Application.Common.Result;
using HealthCare.Application.Errors;
using HealthCare.Application.Features.DoctorAppointments.Contracts;
using HealthCare.Application.Features.LabAppointment.Contracts;
using HealthCare.Application.Interfaces.Repositories.UnitOfWork;
using HealthCare.Application.Services;
using HealthCare.Domain.Entities;
using HealthCare.Domain.Enums;
using HealthCare.Domain.Users;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Error = HealthCare.Application.Common.Result.Error;

namespace HealthCare.Application.Features.DoctorAppointments.Commands.BookDoctorAppointment;

public class BookDoctorAppointmentCommandHandler(
    IUnitOfWork unitOfWork,
    IPaymobService paymobService,
    INotificationService notificationService)
    : IRequestHandler<BookDoctorAppointmentCommand, Result<BookDoctorAppointmentResponse>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IPaymobService _paymobService = paymobService;
    private readonly INotificationService _notificationService = notificationService;

    public async Task<Result<BookDoctorAppointmentResponse>> Handle(BookDoctorAppointmentCommand request, CancellationToken cancellationToken)
    {
        //check Patient exist And get his Data
        var patient = await _unitOfWork.Patients.AsQueryable()
            .Include(p => p.User)
            .Where(p => p.UserId == request.UserId)
            .SingleOrDefaultAsync(cancellationToken);

        if (patient is null)
            return Result.Failure<BookDoctorAppointmentResponse>(UserErrors.NotFound);

        //check doctor Exist And get doctor data
        var doctor = await _unitOfWork.Doctors.AsQueryable()
            .Where(d => d.Id == request.DoctorId)
            .Select(d => new
            {
                d.Id,
                d.User.Email,
                d.User.Name,
                d.User.PhoneNumber,
                d.User.Address,
                d.HomeFee,
                d.OnlineFee,
                d.ClinicFee,
                d.AllowOnlineConsultation,
                d.AllowHomeVisit
            })
            .SingleOrDefaultAsync(cancellationToken);

        if (doctor is null)
            return Result.Failure<BookDoctorAppointmentResponse>(DoctorErrors.NotFound);

        //check if the slot is exist or already booked and get the slot with tracking cuz we might need to update the isbooked
        var slot = await _unitOfWork.DoctorSlots.AsQueryable()
            .Where(ds => ds.Id == request.DoctorSlotId && ds.DoctorId == request.DoctorId)
            .SingleOrDefaultAsync(cancellationToken);

        if (slot is null)
            return Result.Failure<BookDoctorAppointmentResponse>(DoctorSlotsErrors.NotFound);
        
        if(slot.Date.ToDateTime(slot.StartTime) < DateTime.UtcNow)
            return Result.Failure<BookDoctorAppointmentResponse>(new Error(
                "DoctorSlots.PastSlot",
                "This time slot has already passed.",
                400));

        if (slot.IsBooked)
            return Result.Failure<BookDoctorAppointmentResponse>(DoctorAppointmentErrors.DuplicateBooking);

        // get the type of the appointment and chech if the doctor allow this type
        (bool isHomeVisit, bool isOnline, bool isClinic) = GetAppointmentType(request.AppointmentType);

        if (isHomeVisit && !doctor.AllowOnlineConsultation)
            return Result.Failure<BookDoctorAppointmentResponse>(DoctorAppointmentErrors.OnlineNotSupported);

        if (isOnline && !doctor.AllowHomeVisit)
            return Result.Failure<BookDoctorAppointmentResponse>(DoctorAppointmentErrors.HomeVisitNotSupported);

        // create the appointment and save it in the database this happen using transaction to make sure everything is saved or everything not saved
        using var transaction = await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            var appointmentType = Enum.Parse<AppointmentType>(request.AppointmentType, true);
            var fee = isHomeVisit ? doctor.HomeFee : isOnline ? doctor.OnlineFee : doctor.ClinicFee;

            var appointment = new DoctorAppointment
            {
                PatientId = patient.Id,
                DoctorId = doctor.Id,
                DoctorSlotId = slot.Id,
                Notes = request.Notes,
                Address = request.Address,
                AppointmentType = appointmentType,
                Status = isClinic ? AppointmentStatus.Confirmed : AppointmentStatus.Pending,
                PaymentType = isOnline ? PaymentType.Online : PaymentType.Cash,
                PaymentStatus = isOnline ? PaymentStatus.Pending : PaymentStatus.NotRequired,
                Fee = fee
            };

            await _unitOfWork.DoctorAppointments.AddAsync(appointment);
            slot.IsBooked = true;
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            string? checkoutUrl = null;

            // online payment
            if (isOnline)
            {
                var paymentResult = await _paymobService.ProcessPaymentAsync(appointment, patient);

                if (paymentResult.IsFailure)
                {
                    await transaction.RollbackAsync(cancellationToken);
                    return Result.Failure<BookDoctorAppointmentResponse>(paymentResult.Error);
                }
                checkoutUrl = paymentResult.Value.CheckoutUrl;

                // update the appointment
                appointment.PaymentOrderId = paymentResult.Value.IntentionId;
            }
            else
            {
                //send email notfiaciton
                await _notificationService.SendNewAppointmentNotificationAsync(
                    doctor.Email!,
                    doctor.Name,
                    DefaultRoles.Doctor,
                    patient.User.Name,
                    slot.Date.ToString(),
                    appointment.AppointmentType.ToString()
                );

                //create the chat
                var isChatExist = await _unitOfWork.Chats.AnyAsync(c => c.PatientId == patient.Id && c.DoctorId == doctor.Id);
                if (!isChatExist)
                {
                    var chat = new Chat
                    {
                        DoctorId = doctor.Id,
                        PatientId = patient.Id
                    };

                    // add it to database
                    await _unitOfWork.Chats.AddAsync(chat);
                }

            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            var response = new BookDoctorAppointmentResponse(
                appointment.Id,
                doctor.Name,
                doctor.PhoneNumber!,
                slot.Date,
                slot.StartTime,
                slot.EndTime,
                isHomeVisit ? request.Address : isClinic ? doctor.Address : null,
                appointment.AppointmentType,
                appointment.Fee,
                appointment.Status,
                isOnline ? checkoutUrl : null
            );
  
            return Result.Success(response);
        }
        catch (Exception)
        {
            await transaction.RollbackAsync(cancellationToken);
            return Result.Failure<BookDoctorAppointmentResponse>(DoctorAppointmentErrors.SaveFailed);
        }
    }






    private (bool isHomeVisit, bool isOnline, bool isClinic) GetAppointmentType(string type)
    {
        if (type.Equals(nameof(AppointmentType.HomeVisit), StringComparison.OrdinalIgnoreCase))
            return (true, false, false);

        if (type.Equals(nameof(AppointmentType.Online), StringComparison.OrdinalIgnoreCase))
            return (false, true, false);

        return (false, false, true);
    }
}
