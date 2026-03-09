using HealthCare.Application.Common.Pagination;
using HealthCare.Application.Features.Users.Contracts;
using HealthCare.Application.Features.Users.Queries.GetUsers;
using HealthCare.Application.Interfaces.Repositories.Base;
using HealthCare.Domain.Users;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Interfaces.Repositories;

public interface IUserRepository : IBaseRepository<ApplicationUser>
{
    Task<PagedList<UserResponse>> GetUsersAsync(GetUsersQuery query, CancellationToken cancellationToken = default);
}
