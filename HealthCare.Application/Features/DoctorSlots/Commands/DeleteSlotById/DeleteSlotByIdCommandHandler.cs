using HealthCare.Application.Common.Result;
using HealthCare.Application.Errors;
using HealthCare.Application.Interfaces.Repositories.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.DoctorSlots.Commands.DeleteSlotsById;

public class DeleteSlotByIdCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<DeleteSlotByIdCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result> Handle(DeleteSlotByIdCommand request, CancellationToken cancellationToken)
    {
        var doctorId = await _unitOfWork.Doctors.AsQueryable()
            .Where(d => d.UserId == request.UserId)
            .Select(d => d.Id)
            .SingleOrDefaultAsync(cancellationToken);

        if(doctorId == Guid.Empty)
            return Result.Failure(UserErrors.NotFound);

        var slot = await _unitOfWork.DoctorSlots.AsQueryable()
            .Where(ds => ds.DoctorId == doctorId && ds.Id == request.SlotId)
            .SingleOrDefaultAsync(cancellationToken);  

        if(slot is null)
            return Result.Failure(DoctorSlotsErrors.NotFound);

        if(slot.IsBooked)
            return Result.Failure(DoctorSlotsErrors.DeleteBookedSlot);

        await _unitOfWork.DoctorSlots.Delete(slot);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
