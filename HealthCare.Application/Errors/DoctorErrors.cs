using HealthCare.Application.Common.Result;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Errors;

public static class DoctorErrors
{

    public static readonly Error NotFound =
        new("Doctor.NotFound", "Doctor is Not Found", 404);
}
