using HealthCare.Application.Common.Result;
using HealthCare.Application.Errors;
using HealthCare.Application.Interfaces.Repositories.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace HealthCare.Application.Features.DoctorSlots.Commands.DeleteSlots;

public class DeleteSlotsCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<DeleteSlotsCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result> Handle(DeleteSlotsCommand request, CancellationToken cancellationToken)
    {
        var doctorId = await _unitOfWork.Doctors.AsQueryable()
            .Where(d => d.UserId == request.UserId)
            .Select(d => d.Id)
            .SingleOrDefaultAsync(cancellationToken);

        if (doctorId == Guid.Empty)
            return Result.Failure(UserErrors.NotFound);

        var res = await _unitOfWork.DoctorSlots.AsQueryable()
            .Where(ds => ds.DoctorId == doctorId && ds.Date == request.Date && !ds.IsBooked)
            .ExecuteDeleteAsync(cancellationToken);

        return Result.Success();
    }
}
