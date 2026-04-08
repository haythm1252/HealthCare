using HealthCare.Application.Common.Pagination;
using HealthCare.Application.Features.Appointments.Contracts;
using HealthCare.Application.Features.DoctorAppointments.Contracts;
using HealthCare.Application.Features.LabAppointment.Contracts;
using HealthCare.Application.Features.LabAppointment.Queries.GetLabAppointments;
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

public class LabAppointmentRepository(ApplicationDbContext context)
    : BaseRepository<LabAppointment>(context), ILabAppointmentRepository
{
    private readonly ApplicationDbContext _context = context;

    public async Task<IEnumerable<AppointmentDto>> GetPatientAppointmentsAsync(Guid patientId, CancellationToken cancellationToken)
    {
        return await _context.LabAppointments
            .AsNoTracking()
            .Where(a => a.PatientId == patientId)
            .OrderByDescending(a => a.CreatedAt)
            .Select(a => new AppointmentDto
            (
                Id: a.Id,
                ProviderName: a.Lab.User.Name,
                ProviderImage: a.Lab.ProfilePictureUrl,
                Type: TargetType.Lab,
                Date: a.Date,
                Time: a.StartTime,
                Status: a.Status,
                Price: a.TotalFee,
                ServiceType: a.AppointmentType.ToString(),
                ScheduledAt: a.CreatedAt,
                Specialty: null
            ))
            .ToListAsync(cancellationToken);
    }

    public async Task<PagedList<LabAppointmentResponse>> GetLabAppointmentsWithFiltersAsync(Guid labId, GetLabAppointmentsQuery filters, CancellationToken cancellationToken)
    {
        var query = _context.LabAppointments
            .AsNoTracking()
            .Where(a => a.LabId == labId);

        var hasStatus = Enum.TryParse<AppointmentStatus>(filters.Status, true, out var status);
        var hasType = Enum.TryParse<AppointmentType>(filters.AppointmentType, true, out var type);


        if (!string.IsNullOrWhiteSpace(filters.Search))
            query = query.Where(a => a.Patient.User.Name.Contains(filters.Search));

        if (hasStatus)
            query = query.Where(a => a.Status == status);

        if (hasType)
            query = query.Where(a => a.AppointmentType == type);

        bool isPendingHomeVisits = (hasType && type == AppointmentType.HomeVisit) &&
                      (hasStatus && status == AppointmentStatus.Pending);

        return await query.OrderByDescending(a => a.CreatedAt)
            .Select(a => new LabAppointmentResponse
            (
                a.Id,
                a.Patient.User.Name,
                a.Date,
                a.StartTime,
                a.Status,
                a.AppointmentType,
                a.TestResults.Count,
                a.Address,

                // i didnt check by the data of the appointment cuz i want to send this data only if it ask me in the filters to get the bending home visit 
                // (for the pending home visit requests page so i dont make another endpoint 
                isPendingHomeVisits ? a.Patient.User.PhoneNumber : null,
                isPendingHomeVisits ? a.Notes : null,
                isPendingHomeVisits ? a.Patient.User.Gender : null,
                isPendingHomeVisits ? a.TestResults.Select(tr => tr.Test.Name) : null
            ))
            .ToPagedListAsync(filters.Page, filters.PageSize, cancellationToken);

    }
}
