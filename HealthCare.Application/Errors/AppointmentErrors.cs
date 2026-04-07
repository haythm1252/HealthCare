using HealthCare.Application.Common.Result;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Errors;

public static class AppointmentErrors
{
    public static readonly Error NotFound =
        new("Appointment.NotFound", "The requested appointment was not found or you do not have permission.", 404);

    public static readonly Error InvalidStatus =
        new("Appointment.InvalidStatus", "Only pending appointments can be Confirmed.", 400);

    public static readonly Error NotConfirmed =
        new("Appointment.NotConfirmed", "The appointment status is not Confrimed.", 400);
    public static Error TooEarlyToFinalize =>
        new("Appointment.TooEarly", "You cannot finalize an appointment before its scheduled time.", 400);

    public static readonly Error AppointmentExpired =
        new("Appointment.Expired", "The appointment time has passed; it has been marked as Declined.", 400);

    public static readonly Error NotHomeVisit =
        new("Appointment.NotHomeVisit", "The Appointments is Not Home Visit", 400);

    public static readonly Error CancellationNotAllowed = 
        new("Appointment.CancellationNotAllowed", "The time limit for cancelling this appointment has passed.", 400);

    public static readonly Error AlreadyCancelled = 
        new("Appointment.AlreadyCancelled","This appointment has already been cancelled and cannot be modified.", 400);

    public static readonly Error OnlineCancellationForbidden = 
        new("Appointment.OnlineCancellationForbidden", "Online consultations cannot be cancelled once they are confirmed." ,400);

    public static readonly Error UnSupportedType =
        new("Appointment.WrongType", "Unsupported Appointment Type", 400);


    public static readonly Error PaymentNotSupported =
    new("Appointment.PaymentNotSupported", "This appointment is not an Online appointment; online payment is not supported for this appointment type.", 400);
}
