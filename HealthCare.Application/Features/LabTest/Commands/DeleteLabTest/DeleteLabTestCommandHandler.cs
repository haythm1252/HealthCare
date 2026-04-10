using HealthCare.Application.Common.Result;
using HealthCare.Application.Errors;
using HealthCare.Application.Interfaces.Repositories.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.LabTest.Commands.DeleteLabTest;

public class DeleteLabTestCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<DeleteLabTestCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result> Handle(DeleteLabTestCommand request, CancellationToken cancellationToken)
    {
        var labtest = await _unitOfWork.LabTests.AsQueryable()
            .Where(lt => lt.Id == request.LabTestId && lt.Lab.UserId == request.UserId && !lt.IsDeleted)
            .SingleOrDefaultAsync(cancellationToken);

        if (labtest is null)
            return Result.Failure(TestErrors.NotFound);

        labtest.IsDeleted = true;
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
    
}
