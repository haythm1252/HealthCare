using HealthCare.Application.Features.Appointments.Contracts;
using HealthCare.Application.Interfaces.Repositories;
using HealthCare.Domain.Entities;
using HealthCare.Domain.Enums;
using HealthCare.Infrastructure.Persistence;
using HealthCare.Infrastructure.Repositories.Base;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Infrastructure.Repositories;

public class NurseAppointmentRepository(ApplicationDbContext context)
    : BaseRepository<NurseAppointment>(context), INurseAppointmentRepository
{
    private readonly ApplicationDbContext _context = context;

    public async Task<IEnumerable<AppointmentDto>> GetPatientAppointmentsAsync(Guid patientId, CancellationToken cancellationToken)
    {
        return await _context.NurseAppointments
            .AsNoTracking()
            .Where(a => a.PatientId == patientId)
            .OrderByDescending(a => a.CreatedAt)
            .Select(a => new AppointmentDto
            (
                Id: a.Id,
                ProviderName: a.Nurse.User.Name,
                ProviderImage: a.Nurse.ProfilePictureUrl,
                Type: TargetType.Nurse,
                Date: a.NurseShift.Date,
                Time: a.StartTime,
                Status: a.Status,
                Price: a.TotalFee,
                ServiceType: a.ServiceType.ToString(),
                ScheduledAt: a.CreatedAt,
                Specialty: null
            ))
            .ToListAsync(cancellationToken);
    }
}
