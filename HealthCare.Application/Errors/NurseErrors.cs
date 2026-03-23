using HealthCare.Application.Common.Result;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Errors;

public static class NurseErrors
{
    public static readonly Error NotFound =
        new("Nurse.NotFound", "Nurse is Not Found", 404);
}
