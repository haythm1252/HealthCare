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

public class AddTestCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<AddTestCommand, TestResponse>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<TestResponse> Handle(AddTestCommand request, CancellationToken cancellationToken)
    {
        var test = new Test
        {
            Name = request.Name,
            Description = request.Description,
            PreRequisites = request.PreRequisites,
        };

        await _unitOfWork.Tests.AddAsync(test,cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return test.Adapt<TestResponse>();
    }
}
