using HealthCare.Application.Common.Result;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Errors;

public static class NurseAppointmentErrors
{
    public static readonly Error ShiftNotAvailable =
            new("NurseAppointment.ShiftNotAvailable", "The selected shift is either outdated or doesn't belong to this nurse.", 400);

    public static readonly Error DuplicateBooking =
        new ("NurseAppointment.DuplicateBooking", "You have already submitted a booking request for this specific shift.", 409);

    public static readonly Error SaveFailed =
        new("NurseAppointment.SaveFailed", "An error occurred while saving the appointment. Please try again.", 500);
}
