using HealthCare.Application.Common.Consts;
using HealthCare.Application.Common.Result;
using HealthCare.Application.Errors;
using HealthCare.Application.Interfaces.Repositories.UnitOfWork;
using HealthCare.Application.Services;
using HealthCare.Domain.Entities;
using HealthCare.Domain.Enums;
using HealthCare.Domain.Users;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace HealthCare.Application.Features.Payment.Commands.PaymentCallback;

public class PaymentCallbackCommandHandler(
    IUnitOfWork unitOfWork,
    IPaymobService paymobService,
    INotificationService notificationService,
    ILogger<PaymentCallbackCommandHandler> logger)
    : IRequestHandler<PaymentCallbackCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IPaymobService _paymobService = paymobService;
    private readonly INotificationService _notificationService = notificationService;
    private readonly ILogger<PaymentCallbackCommandHandler> _logger = logger;

    public async Task<Result> Handle(PaymentCallbackCommand request, CancellationToken cancellationToken)
    {
        // Verify HMAC
        if (!_paymobService.IsValidSignature(request.Payload, request.Signature))
        {
            _logger.LogWarning("Invalid HMAC signature received for payment callback.");
            return Result.Failure(PaymentErrors.PaymentProviderFailed);
        }

        // get the data
        if (!request.Payload.TryGetProperty("obj", out var obj))
        {
            _logger.LogWarning("Payment callback payload missing 'obj'.");
            return Result.Failure(PaymentErrors.PaymentProviderFailed);
        }

        var success = obj.GetProperty("success").GetBoolean();

        var orderElement = obj.GetProperty("order");
        var merchantIdString = orderElement.GetProperty("merchant_order_id").GetString();

        if (!Guid.TryParse(merchantIdString, out var appointmentId))
        {
            _logger.LogWarning("Invalid merchant_order_id in payment callback: {MerchantId}", merchantIdString);
            return Result.Failure(PaymentErrors.PaymentProviderFailed);
        }

        var appointment = await _unitOfWork.DoctorAppointments.AsQueryable()
            .Include(da => da.DoctorSlot)
            .Where(da => da.Id == appointmentId)
            .SingleOrDefaultAsync(cancellationToken);

        if (appointment is null)
        {
            _logger.LogWarning("Appointment not found for payment callback: {AppointmentId}", appointmentId);
            return Result.Failure(DoctorErrors.NotFound);
        }

        // check if we already update the payment before
        if (appointment.PaymentStatus == PaymentStatus.Paid)
        {
            _logger.LogInformation("Payment callback received but appointment already marked as Paid: {AppointmentId}", appointmentId);
            return Result.Success();
        }


        // update status and send email and create chat
        if (success)
        {

            appointment.PaymentStatus = PaymentStatus.Paid;
            appointment.Status = AppointmentStatus.Confirmed;
            appointment.DoctorSlot.IsBooked = true;
            appointment.PaymentTransactionId = obj.GetProperty("id").ToString();

            var emailData = await _unitOfWork.DoctorAppointments.AsQueryable()
                .Where(da => da.Id == appointment.Id)
                .Select(da => new
                {
                    DoctorEmail = da.Doctor.User.Email,
                    DoctorName = da.Doctor.User.Name,
                    PatientName = da.Patient.User.Name,
                    PatientEmail = da.Patient.User.Email,

                }).SingleOrDefaultAsync(cancellationToken);

            //send email notificaiton to doctor
            await _notificationService.SendNewAppointmentNotificationAsync(
                    emailData!.DoctorEmail!,
                    emailData!.DoctorName,
                    DefaultRoles.Doctor,
                    emailData!.PatientName,
                    appointment.DoctorSlot.Date.ToString(),
                    $"{appointment.AppointmentType.ToString()} Appointment And the patient Successfully Paid"
                );

            //send email notificton to patient
            await _notificationService.SendPaymentConfrimationNotificationAsync(
                    emailData!.PatientEmail!,
                    emailData!.PatientName,
                    emailData!.DoctorName,
                    appointment.DoctorSlot.Date,
                    appointment.DoctorSlot.StartTime,
                    appointment.DoctorSlot.EndTime,
                    appointment.AppointmentType.ToString()
                );

            //create the chat
            var isChatExist = await _unitOfWork.Chats.AnyAsync(c => c.PatientId == appointment.PatientId && c.DoctorId == appointment.DoctorId);
            if (!isChatExist)
            {
                var chat = new Chat
                {
                    DoctorId = appointment.DoctorId,
                    PatientId = appointment.PatientId
                };

                await _unitOfWork.Chats.AddAsync(chat);
            }
        }
        else
        {
            appointment.PaymentStatus = PaymentStatus.Failed;
            appointment.DoctorSlot.IsBooked = false;
            _logger.LogInformation("Payment failed for appointment {AppointmentId}", appointmentId);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Payment callback processed successfully for appointment {AppointmentId}", appointmentId);
        return Result.Success();
    }
}