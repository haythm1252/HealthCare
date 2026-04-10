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
        var test = await _unitOfWork.Tests.AsQueryable()
            .Include(t => t.LabTests)
            .Where(t => t.Id == request.Id)
            .SingleOrDefaultAsync(cancellationToken);

        if (test is null)
            return Result.Failure(TestErrors.NotFound);

        test.IsDeleted = true;

        foreach (var labTest in test.LabTests)
            labTest.IsDeleted = true;

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
