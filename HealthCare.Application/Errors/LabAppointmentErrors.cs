using HealthCare.Application.Common.Result;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Errors;

public static class LabAppointmentErrors
{
    public static readonly Error TestsNotExisit =
        new("LabAppointment.TestsNotExisit", "The selected tests is either not exist or this lab dosen't support it.", 400);

    public static readonly Error ClosedOnSelectedDay =
        new("LabAppointment.ClosedOnSelectedDay", "The lab is closed on the selected date.", 409);

    public static readonly Error NotSupportHome =
    new("LabAppointment.NotSupportHome", "Some of the selected tests not support HomeVisit Service", 400);

    public static readonly Error DuplicateBooking =
        new("LabAppointment.DuplicateBooking", "You have already submitted an ongoing booking request for this lab in this day " +
            "if you want to change something in the request you can cancele it and book another appointment", 409);

    public static readonly Error SaveFailed =
        new("LabAppointment.SaveFailed", "An error occurred while saving the appointment. Please try again.", 500);
}
