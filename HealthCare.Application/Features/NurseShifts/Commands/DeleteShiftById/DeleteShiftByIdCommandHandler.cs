using HealthCare.Application.Common.Result;
using HealthCare.Application.Errors;
using HealthCare.Application.Interfaces.Repositories.UnitOfWork;
using HealthCare.Domain.Users;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.NurseShifts.Commands.DeleteShiftById;

public class DeleteShiftByIdCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<DeleteShiftByIdCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result> Handle(DeleteShiftByIdCommand request, CancellationToken cancellationToken)
    {
        var nurseId = await _unitOfWork.Nurses.AsQueryable()
            .Where(n => n.UserId == request.UserId)
            .Select(n => n.Id)
            .SingleOrDefaultAsync(cancellationToken);

        if (nurseId == Guid.Empty)
            return Result.Failure(UserErrors.NotFound);

        var shift = await _unitOfWork.NurseShifts.AsQueryable()
            .Where(ns => ns.NurseId == nurseId && ns.Id == request.ShiftId)
            .SingleOrDefaultAsync(cancellationToken);

        if (shift is null)
            return Result.Failure(NurseShiftsErrors.NotFound);

        if (shift.IsBooked)
            return Result.Failure(NurseShiftsErrors.DeleteBookedShift);

        await _unitOfWork.NurseShifts.Delete(shift);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
