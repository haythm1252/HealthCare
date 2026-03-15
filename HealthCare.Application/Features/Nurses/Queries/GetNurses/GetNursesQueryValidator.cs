using FluentValidation;
using HealthCare.Application.Common.Consts;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.Nurses.Queries.GetNurses;

public class GetNursesQueryValidator : AbstractValidator<GetNursesQuery>
{
    public GetNursesQueryValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThan(0)
            .WithMessage("Page must be greater than 0.");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 50)
            .WithMessage("PageSize must be between 1 and 50.");

        RuleFor(x => x.MinRate)
            .InclusiveBetween(0, 5)
            .When(x => x.MinRate.HasValue)
            .WithMessage("MinRate must be between 0 and 5.");

        RuleFor(x => x.City)
            .Must(city => EgyptGovernorates.IsValid(city!))
            .When(x => !string.IsNullOrWhiteSpace(x.City))
            .WithMessage("Invalid governorate, please select a valid governorate.");

        RuleFor(x => x.Sort)
            .Must(x => FiltersOptions.SortFilters.Contains(x!))
            .When(x => !string.IsNullOrEmpty(x.Sort))
            .WithMessage($"Invalid sort value. please chose one of those filters {string.Join(",", FiltersOptions.SortFilters)}");
    }
}
