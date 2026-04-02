using HealthCare.Application.Common.Result;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Errors;

public static class ReviewErrors
{
    public static readonly Error TargetNotFound =
        new("Review.TargetNotFound", "The review Target Not Found, Target is may be doctor, nurse, lab", 400);

    public static readonly Error IvalidTargetType =
        new("Review.IvalidTargetType", "The target type is invalid chose one of this types Doctor, Nurse, Lab", 400);

    public static readonly Error NoAppointmentExist =
        new("Review.NoAppointmentExist", "The Patient dose not have appointment with Target", 409);
}
