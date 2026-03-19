using HealthCare.Application.Common.Result;
using HealthCare.Application.Errors;
using HealthCare.Application.Interfaces.Repositories.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using System.Text;

namespace HealthCare.Application.Features.Tests.Commands.DeleteTest;

public class DeleteTestCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<DeleteTestCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result> Handle(DeleteTestCommand request, CancellationToken cancellationToken)
    {
        var res = await _unitOfWork.Tests.AsQueryable()
            .Where(t => t.Id == request.Id)
            .ExecuteDeleteAsync(cancellationToken);

        return res > 0 ? Result.Success() :
            Result.Failure(TestErrros.NotFound);
    }
}
