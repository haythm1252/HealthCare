using HealthCare.Application.Common.Result;
using HealthCare.Application.Features.Tests.Contracts;
using HealthCare.Application.Interfaces.Repositories;
using HealthCare.Application.Interfaces.Repositories.UnitOfWork;
using HealthCare.Domain.Entities;
using Mapster;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.Tests.Commands.AddTest;

public class AddTestCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<AddTestCommand, Result<TestResponse>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result<TestResponse>> Handle(AddTestCommand request, CancellationToken cancellationToken)
    {
        var isTestExist = await _unitOfWork.Tests.AnyAsync(t => t.Name.ToLower() == request.Name.ToLower() && !t.IsDeleted, cancellationToken);
        if (isTestExist)
            return Result.Failure<TestResponse>(new Error("Test.AlreadyExist", "The Test you tring to add is already exist", 409));
        var test = new Test
        {
            Name = request.Name,
            Description = request.Description,
            PreRequisites = request.PreRequisites,
        };

        await _unitOfWork.Tests.AddAsync(test,cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(test.Adapt<TestResponse>());
    }
}
