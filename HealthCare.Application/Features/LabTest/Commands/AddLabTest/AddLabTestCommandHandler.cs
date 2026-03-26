using HealthCare.Application.Common.Result;
using HealthCare.Application.Errors;
using HealthCare.Application.Features.LabTest.Contracts;
using HealthCare.Application.Interfaces.Repositories.UnitOfWork;
using HealthCare.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HealthCare.Application.Features.LabTest.Commands.AddLabTest;

public class AddLabTestCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<AddLabTestCommand, Result<LabTestResponse>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result<LabTestResponse>> Handle(AddLabTestCommand request, CancellationToken cancellationToken)
    {
        var labId = await _unitOfWork.Labs.AsQueryable()
            .Where(l => l.UserId == request.UserId)
            .Select(l => l.Id)
            .SingleOrDefaultAsync(cancellationToken);

        if(labId == Guid.Empty)
            return Result.Failure<LabTestResponse>(UserErrors.NotFound);

        var test = await _unitOfWork.Tests.AsQueryable()
            .Where(l => l.Id == request.TestId)
            .SingleOrDefaultAsync(cancellationToken);

        if(test is null)
            return Result.Failure<LabTestResponse>(TestErrros.NotFound);

        var isAddedAlready = await _unitOfWork.LabTests.AnyAsync(lt => lt.TestId == request.TestId, cancellationToken);
        if(isAddedAlready)
            return Result.Failure<LabTestResponse>(new Error("LabTest.AlreadyExist",
                "the test you want to add is already exist", 409));

        var labTest = new Domain.Entities.LabTest
        {
            LabId = labId,
            TestId = request.TestId,
            Price = request.Price,
            IsAvailableAtHome = request.IsAvailableAtHome
        };

        await _unitOfWork.LabTests.AddAsync(labTest, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var response = new LabTestResponse(
                labTest.Id,
                test.Id,
                test.Name,
                test.Description,
                test.PreRequisites,
                labTest.Price,
                labTest.IsAvailableAtHome
            );

        return Result.Success(response);
    }
}
