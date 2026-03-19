using HealthCare.Application.Common.Result;
using HealthCare.Application.Errors;
using HealthCare.Application.Features.Doctors.Contracts;
using HealthCare.Application.Interfaces.Repositories.UnitOfWork;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.Nurses.Commands.UpdatePricing;

public class UpdateNursePricingCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<UpdateNursePricingCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result> Handle(UpdateNursePricingCommand request, CancellationToken cancellationToken)
    {
        var nurse = await _unitOfWork.Nurses.AsQueryable()
            .SingleOrDefaultAsync(n => n.UserId == request.UserId, cancellationToken);
        
        if (nurse is null)
            return Result.Failure(UserErrors.NotFound);

        request.Adapt(nurse);
        await _unitOfWork.SaveChangesAsync(cancellationToken);  

        return Result.Success();
    }
}
