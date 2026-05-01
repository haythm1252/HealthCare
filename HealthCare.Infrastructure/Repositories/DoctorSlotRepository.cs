using HealthCare.Application.Interfaces.Repositories;
using HealthCare.Domain.Entities;
using HealthCare.Infrastructure.Persistence;
using HealthCare.Infrastructure.Repositories.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Infrastructure.Repositories;

public class DoctorSlotRepository(ApplicationDbContext context) : BaseRepository<DoctorSlot>(context), IDoctorSlotRepository
{
    private readonly ApplicationDbContext _context = context;

    public async Task<(int slotsToDel, int slotsDeleted)> DeleteSlots()
    {
        var slotsIds = await _context.DoctorSlots
            .AsNoTracking()
            .Where(s => s.Date < DateOnly.FromDateTime(DateTime.UtcNow) && !s.IsBooked && !s.DoctorAppointments.Any())
            .Select(s => s.Id)
            .ToListAsync();

        var res = await _context.DoctorSlots
            .Where(s => slotsIds.Contains(s.Id))
            .ExecuteDeleteAsync();

        return (slotsIds.Count, res);
    }
}
