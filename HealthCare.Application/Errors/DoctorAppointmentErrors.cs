using HealthCare.Application.Common.Result;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Errors;

public static class DoctorAppointmentErrors
{
    public static readonly Error DoctorOrSlotNotFound =
        new("DoctorAppointment.DoctorOrSlotNotFound", "The Doctor or the Slot that you chosed are not Found", 400);

    public static readonly Error HomeVisitNotSupported=
        new("DoctorAppointment.HomeVisitNotSupported", "The doctor you chose dose Not Allow the HomeVisit Appointment", 400);

    public static readonly Error OnlineNotSupported =
        new("DoctorAppointment.OnlineNotSupported", "The doctor you chose dose Not Allow the Online Appointment", 400);

    public static Error TooEarlyToAddDiagnosis =>
        new("DoctorAppointment.TooEarlyToAddDiagnosis", "You cannot add diagnosis to an appointment before its scheduled time.", 400);

    public static readonly Error DuplicateBooking =
        new("DoctorAppointment.DuplicateBooking", "the Doctor slot you select is Already Booked ", 400);

    public static readonly Error SaveFailed =
        new("DoctorAppointment.SaveFailed", "An error occurred while saving the appointment. Please try again.", 500);
}
