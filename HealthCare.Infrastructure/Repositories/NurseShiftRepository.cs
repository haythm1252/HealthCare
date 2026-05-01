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

public class NurseShiftRepository(ApplicationDbContext context) : BaseRepository<NurseShift>(context), INurseShiftRepository
{
    private readonly ApplicationDbContext _context = context;

    public async Task<(int shiftsToDel, int shiftsDeleted)> DeleteShifts()
    {
        var shiftsIds = await _context.NurseShifts
            .Where(s => s.Date < DateOnly.FromDateTime(DateTime.UtcNow) && !s.IsBooked && !s.NurseAppointments.Any())
            .Select(s => s.Id)
            .ToListAsync();

        var res = await _context.NurseShifts
            .Where(s => shiftsIds.Contains(s.Id))
            .ExecuteDeleteAsync();

        return (shiftsIds.Count, res);
    }
}
