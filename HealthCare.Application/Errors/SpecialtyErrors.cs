using HealthCare.Application.Common.Result;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Errors;

public static class SpecialtyErrors
{
    public static readonly Error NotFound =
    new("Specialty.NotFound", "Specialty is Not Found", 400);
}
