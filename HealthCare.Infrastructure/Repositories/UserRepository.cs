using Azure.Core;
using HealthCare.Application.Common.Consts;
using HealthCare.Application.Common.Pagination;
using HealthCare.Application.Features.Users.Contracts;
using HealthCare.Application.Features.Users.Queries.GetUsers;
using HealthCare.Application.Interfaces.Repositories;
using HealthCare.Domain.Users;
using HealthCare.Infrastructure.Persistence;
using HealthCare.Infrastructure.Repositories.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Infrastructure.Repositories;

public class UserRepository(ApplicationDbContext context) : BaseRepository<ApplicationUser>(context), IUserRepository
{
    private readonly ApplicationDbContext _context = context;

    public async Task<PagedList<UserResponse>> GetUsersAsync(GetUsersQuery query, CancellationToken cancellationToken = default)
    {
        var users = _context.Users.AsQueryable();

        if(query.Search is not null)
            users = users.Where(u => u.Name.Contains(query.Search) || u.Email!.Contains(query.Search) || u.Id == query.Search);

        users = query.Descending 
            ? users.OrderByDescending(u => u.Name) 
            : users.OrderBy(u => u.Name);

        if(query.IsDisabled)
            users = users.Where(u => u.IsDisabled);

        var usersQuery = users
                .Join(_context.UserRoles, u => u.Id, ur => ur.UserId, (u, ur) => new { User = u, UserRole = ur })
                .Join(_context.Roles, ur => ur.UserRole.RoleId, r => r.Id, (ur, r) => new { ur.User, Role = r });

        usersQuery = string.IsNullOrWhiteSpace(query.Role)
            ? usersQuery.Where(ur => ur.Role.Name != DefaultRoles.Admin)
            : usersQuery.Where(ur => ur.Role.Name == query.Role);

        return await usersQuery.Select(ur => new UserResponse
                (
                    Id: ur.User.Id,
                    Name: ur.User.Name,
                    Email: ur.User.Email!,
                    Role: ur.Role.Name!,
                    IsDisabled: ur.User.IsDisabled

                )).ToPagedListAsync(query.PageNumber, query.PageSize, cancellationToken);
    }
}
