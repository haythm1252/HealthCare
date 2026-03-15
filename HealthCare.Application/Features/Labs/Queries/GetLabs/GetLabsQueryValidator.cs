using FluentValidation;
using HealthCare.Application.Common.Consts;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.Labs.Queries.GetLabs;

public class GetLabsQueryValidator : AbstractValidator<GetLabsQuery>
{
    public GetLabsQueryValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThan(0)
            .WithMessage("Page must be greater than 0.");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 50)
            .WithMessage("PageSize must be between 1 and 50.");


        RuleFor(x => x.City)
            .Must(city => EgyptGovernorates.IsValid(city!))
            .When(x => !string.IsNullOrWhiteSpace(x.City))
            .WithMessage("Invalid governorate, please select a valid governorate.");

        RuleFor(x => x.MinRate)
            .InclusiveBetween(0, 5)
            .When(x => x.MinRate.HasValue)
            .WithMessage("MinRate must be between 0 and 5.");

        RuleFor(x => x.Sort)
            .Must(x => x == FiltersOptions.RateAsc || x == FiltersOptions.RateDesc)
            .When(x => !string.IsNullOrEmpty(x.Sort))
            .WithMessage($"Invalid sort value. please chose one of those filters {FiltersOptions.RateAsc}, {FiltersOptions.RateDesc}");
    }
}
