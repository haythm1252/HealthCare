using HealthCare.Application.Common.Pagination;
using HealthCare.Application.Features.Nurses.Contracts;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.Nurses.Queries.GetNurses;

public record GetNursesQuery
(
    string? Search,
    string? City,
    decimal? MinRate,
    string? Sort,
    int Page = 1,
    int PageSize = 20
) : IRequest<PagedList<NurseResponse>>;
