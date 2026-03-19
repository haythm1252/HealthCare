using HealthCare.Application.Common.Result;
using HealthCare.Application.Errors;
using HealthCare.Application.Features.Doctors.Contracts;
using HealthCare.Application.Features.DoctorSlots.Contracts;
using HealthCare.Application.Interfaces.Repositories.UnitOfWork;
using HealthCare.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HealthCare.Application.Features.DoctorSlots.Commands.GenerateSlots;

public class GenerateDoctorSlotsCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GenerateDoctorSlotsCommand, Result<IEnumerable<DailySlotsDto>>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result<IEnumerable<DailySlotsDto>>> Handle(GenerateDoctorSlotsCommand request, CancellationToken cancellationToken)
    {
        // Get doctor id
        var doctorId = await _unitOfWork.Doctors.AsQueryable()
            .Where(d => d.UserId == request.UserId)
            .Select(d => d.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (doctorId == Guid.Empty)
            return Result.Failure<IEnumerable<DailySlotsDto>>(UserErrors.NotFound);

        var effectiveEndDate = request.EndDate ?? request.StartDate;
        var slotDuration = request.ConsultationDurationInminutes;
        var requestedEndTime = request.EndTime ?? request.StartTime.AddMinutes(slotDuration);

        // Delete existing unbooked slots in the time in the during the time in the reqeust
        await _unitOfWork.DoctorSlots.AsQueryable()
            .Where(ds => ds.DoctorId == doctorId
                      && ds.Date >= request.StartDate
                      && ds.Date <= effectiveEndDate
                      && !ds.IsBooked
                      && ds.StartTime < requestedEndTime
                      && ds.EndTime > request.StartTime)
            .ExecuteDeleteAsync(cancellationToken);

        // get the booked slots to not create slot in the same time
        var bookedSlots = await _unitOfWork.DoctorSlots.AsQueryable()
            .Where(ds => ds.DoctorId == doctorId
                      && ds.Date >= request.StartDate
                      && ds.Date <= effectiveEndDate
                      && ds.IsBooked
                      && ds.StartTime < requestedEndTime
                      && ds.EndTime > request.StartTime)
            .ToListAsync(cancellationToken);

        var slotsToGenerate = new List<DoctorSlot>();

        // Generate slots day by day
        for (var date = request.StartDate; date <= effectiveEndDate; date = date.AddDays(1))
            // Generate slots from time x to time y 
            for (var start = request.StartTime; start.AddMinutes(slotDuration) <= requestedEndTime; start = start.AddMinutes(slotDuration))
            {
                var end = start.AddMinutes(slotDuration);

                // see if there is a booked slot in this time
                bool hasConflict = bookedSlots.Any(b => b.Date == date && start < b.EndTime && end > b.StartTime);

                if (!hasConflict)
                    slotsToGenerate.Add(new DoctorSlot
                    {
                        DoctorId = doctorId,
                        Date = date,
                        StartTime = start,
                        EndTime = end,
                        IsBooked = false
                    });
            }

        IEnumerable<DoctorSlot> savedSlots = [];

        if (slotsToGenerate.Count != 0)
        {
            savedSlots = await _unitOfWork.DoctorSlots.AddRangeAsync(slotsToGenerate, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        var response = savedSlots
            .GroupBy(s => s.Date)
            .OrderBy(g => g.Key)
            .Select(g => new DailySlotsDto(
                g.Key,
                g.Key.DayOfWeek.ToString(),
                g.OrderBy(s => s.StartTime)
                 .Select(s => new SlotDto(s.Id, s.StartTime, s.EndTime, s.IsBooked))
                 .ToList()
            ));

        return Result.Success(response);
    }
}