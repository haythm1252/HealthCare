using HealthCare.Application.Common.Result;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Errors;

public static class LabErrors
{
    public static readonly Error NotFound =
        new("Lab.NotFound", "Lab is Not Found", 404);
}
