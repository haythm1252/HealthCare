using HealthCare.Application.Common.Pagination;
using HealthCare.Application.Features.Labs.Contracts;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.Labs.Queries.GetLabs;

public record GetLabsQuery
(
    Guid? TestId,
    string? Search,
    string? City,
    decimal? MinRate,
    string? Sort,
    int Page = 1,
    int PageSize = 20
) : IRequest<PagedList<LabResponse>>;
