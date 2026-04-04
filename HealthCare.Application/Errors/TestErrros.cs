using HealthCare.Application.Common.Result;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Errors;

public static class TestErrros
{
    public static readonly Error NotFound =
        new("Test.NotFound", "Test is Not Found", 404);

    public static readonly Error InvalidTest =
        new("Test.NotFound", "One or more of the selected tests do not exist in the Tests catalog.", 400);
}
