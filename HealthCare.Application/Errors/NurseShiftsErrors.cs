using HealthCare.Application.Common.Result;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Errors;

public class NurseShiftsErrors
{
    public static readonly Error NotFound =
    new("NurseShifts.NotFound", "NurseShifts is Not Found", 400);

    public static readonly Error DeleteBookedShift =
        new("NurseShifts.DeleteBookedShift", "Cannot delete a shift that already booked by patients.", 400);
}
