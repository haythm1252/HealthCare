using HealthCare.Application.Common.Pagination;
using HealthCare.Application.Features.Appointments.Contracts;
using HealthCare.Application.Features.DoctorAppointments.Contracts;
using HealthCare.Application.Features.NurseAppointments.Contracts;
using HealthCare.Application.Features.NurseAppointments.Queries.GetNurseAppointments;
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

    public async Task<PagedList<NurseAppointmentResponse>> GetNurseAppointmentsWithFiltersAsync(Guid nurseId, GetNurseAppointmentsQuery filters, CancellationToken cancellationToken)
    {
        var query = _context.NurseAppointments
        .AsNoTracking()
        .Where(a => a.NurseId == nurseId);

        var hasStatus = Enum.TryParse<AppointmentStatus>(filters.Status, true, out var status);
        var hasType = Enum.TryParse<NurseServiceType>(filters.AppointmentType, true, out var type);

        if (!string.IsNullOrWhiteSpace(filters.Search))
            query = query.Where(a => a.Patient.User.Name.Contains(filters.Search));

        if (hasStatus) 
            query = query.Where(a => a.Status == status);

        if (hasType) 
            query = query.Where(a => a.ServiceType == type);

        bool isPending = (hasStatus && status == AppointmentStatus.Pending);

        return await query.OrderByDescending(a => a.CreatedAt)
            .Select(a => new NurseAppointmentResponse
            (
                a.Id,
                a.Patient.User.Name,
                a.NurseShift.Date,
                a.NurseShift.StartTime,
                a.NurseShift.EndTime,
                a.StartTime,
                a.Status,
                a.ServiceType,
                a.Address,

                a.ServiceType == NurseServiceType.HourlyStay ? a.Hours : null,

                // i didnt check by the data of the appointment cuz i want to send this data only if it ask me in the filters to get the bending home visit 
                // (for the pending home visit requests page so i dont make another endpoint 
                isPending ? a.Patient.User.PhoneNumber : null,
                isPending ? a.Notes : null,
                isPending ? a.Patient.User.Gender : null
            ))
            .ToPagedListAsync(filters.Page, filters.PageSize, cancellationToken);
    }
}
