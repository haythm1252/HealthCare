using HealthCare.Application.Common.Result;
using HealthCare.Application.Errors;
using HealthCare.Application.Features.LabTest.Contracts;
using HealthCare.Application.Interfaces.Repositories.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.LabTest.Commands.UpdateLabTest;

public class UpdateLabTestCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<UpdateLabTestCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result> Handle(UpdateLabTestCommand request, CancellationToken cancellationToken)
    {
        var labId = await _unitOfWork.Labs.AsQueryable()
            .Where(l => l.UserId == request.UserId)
            .Select(l => l.Id)
            .SingleOrDefaultAsync(cancellationToken);

        if (labId == Guid.Empty)
            return Result.Failure(UserErrors.NotFound);

        var labTest = await _unitOfWork.LabTests.AsQueryable()
            .Where(lt => lt.Id == request.LabTestId && lt.LabId == labId)
            .SingleOrDefaultAsync(cancellationToken);

        if (labTest is null)
            return Result.Failure(TestErrors.NotFound);

        labTest.Price = request.Price;
        labTest.IsAvailableAtHome = request.IsAvailableAtHome;

        await _unitOfWork.SaveChangesAsync(cancellationToken); 

        return Result.Success();
    }
}
