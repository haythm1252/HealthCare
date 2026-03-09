using HealthCare.Application.Common.Pagination;
using HealthCare.Application.Common.Result;
using HealthCare.Application.Features.Users.Contracts;
using HealthCare.Application.Interfaces.Repositories.UnitOfWork;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.Users.Queries.GetUsers;

public class GetUsersQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetUsersQuery, PagedList<UserResponse>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<PagedList<UserResponse>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        return await _unitOfWork.Users.GetUsersAsync(request, cancellationToken);
    }
}
