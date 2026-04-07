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

public class DoctorAppointmentRepository(ApplicationDbContext context) 
    : BaseRepository<DoctorAppointment>(context), IDoctorAppointmentRepository
{
    private readonly ApplicationDbContext _context = context;

    public async Task<IEnumerable<AppointmentDto>> GetPatientAppointmentsAsync(Guid patientId, CancellationToken cancellationToken)
    {
        return await _context.DoctorAppointments
            .AsNoTracking()
            .Where(a => a.PatientId == patientId)
            .OrderByDescending(a => a.CreatedAt)
            .Select(a => new AppointmentDto
            (
                Id: a.Id,
                ProviderName: a.Doctor.User.Name,
                ProviderImage: a.Doctor.ProfilePictureUrl,
                Type: TargetType.Doctor,
                Date: a.DoctorSlot.Date,
                Time: a.DoctorSlot.StartTime,
                Status: a.Status,
                Price: a.Fee,
                ServiceType: a.AppointmentType.ToString(),
                ScheduledAt: a.CreatedAt,
                Specialty: a.Doctor.Specialty.Name
            ))
            .ToListAsync(cancellationToken);
    }
      
}
