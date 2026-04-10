using HealthCare.Application.Common.Result;
using HealthCare.Application.Errors;
using HealthCare.Application.Interfaces.Repositories.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.Tests.Commands.UpdateTest;

public class UpdateTestCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<UpdateTestCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result> Handle(UpdateTestCommand request, CancellationToken cancellationToken)
    {
        var test = await _unitOfWork.Tests.AsQueryable()
            .SingleOrDefaultAsync(t => t.Id == request.Id, cancellationToken: cancellationToken);

        if (test is null)
            return Result.Failure(TestErrors.NotFound);

        test.Name = request.Name;
        test.Description = request.Description;
        test.PreRequisites = request.PreRequisites;

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
