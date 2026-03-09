using FluentValidation;
using HealthCare.Application.Common.Consts;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.Users.Queries.GetUsers;

public class GetUsersQueryValidator : AbstractValidator<GetUsersQuery>
{
    public GetUsersQueryValidator()
    { 
        RuleFor(x => x.Search)
            .MaximumLength(250);

        RuleFor(x => x.Role)
            .Must(role => role is null || (DefaultRoles.AllRoles.Contains(role) && role != DefaultRoles.Admin))
            .WithMessage($"Role must be one of the following: {DefaultRoles.Patient}, {string.Join(", ", DefaultRoles.MedicalStaffRoles)} (case sensitive).");


        RuleFor(x => x.PageNumber)
            .GreaterThan(0);

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 50);



    }
}
