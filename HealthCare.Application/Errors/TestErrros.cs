using HealthCare.Application.Common.Result;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Errors;

public static class TestErrros
{
    public static readonly Error NotFound =
        new("Test.NotFound", "Test is Not Found", 404);
}
