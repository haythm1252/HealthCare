using HealthCare.Application.Common.Result;
using HealthCare.Application.Interfaces.Repositories.UnitOfWork;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Errors;

public static class SpecialtyErrors
{
    public static readonly Error NotFound =
        new("Specialty.NotFound", "Specialty is Not Found", 400);

    public static readonly Error AlreadyExists = 
        new ("Specialties.AlreadyExists", "The Speciality you trying to add is already exists", 409);

    public static readonly Error ContainDoctorOrPosts =
        new("Specialties.ContainDoctor", "Can't delete Speciality is Contains Doctors or Posts", 409);
}
