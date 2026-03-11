using HealthCare.Application.Common.Result;
using HealthCare.Application.Errors;
using HealthCare.Application.Interfaces.Repositories.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.Users.Commands.BlockUser;

public class ToggleUserStatusCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<ToggleUserStatusCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result> Handle(ToggleUserStatusCommand request, CancellationToken cancellationToken)
    {
        var res = await _unitOfWork.Users
            .AsQueryable()
            .Where(u => u.Id == request.UserId)
            .ExecuteUpdateAsync(
                setters => setters.SetProperty(u => u.IsDisabled, u => !u.IsDisabled),
                cancellationToken
            );

        return res > 0
            ? Result.Success()
            : Result.Failure(UserErrors.NotFound);
    }
}
