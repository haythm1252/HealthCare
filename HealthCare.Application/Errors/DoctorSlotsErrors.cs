using HealthCare.Application.Common.Result;
using System;
using System.Collections.Generic;
using System.Text;


namespace HealthCare.Application.Errors;

public static class DoctorSlotsErrors
{
    public static readonly Error DoctorSlotEndDateExceeded =
        new("DoctorSlot.SlotEndDateExceeded", "Cannot generate slots beyond 30 days from today.", 400);

    public static readonly Error NotFound =
        new("DoctorSlot.NotFound", "DoctorSlot is Not Found", 400);

    public static readonly Error DeleteBookedSlot =
        new("DoctorSlot.DeleteBookedSlot", "Cannot delete a slot that already booked by patients.", 400);
}
