using HealthCare.Application.Common.Pagination;
using HealthCare.Application.Common.Result;
using HealthCare.Application.Features.Users.Contracts;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.Users.Queries.GetUsers;

public record GetUsersQuery(
    string? Search,
    string? Role,
    bool IsDisabled = false,
    int PageNumber = 1,
    int PageSize = 20
) : IRequest<PagedList<UserResponse>>;
