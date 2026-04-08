using HealthCare.Application.Common.Pagination;
using HealthCare.Application.Features.Appointments.Contracts;
using HealthCare.Application.Features.DoctorAppointments.Contracts;
using HealthCare.Application.Features.DoctorAppointments.Queries.GetDoctorAppointments;
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


    public async Task<PagedList<DoctorAppointmentResponse>> GetDoctorAppointmentsWithFiltersAsync(Guid doctorId, GetDoctorAppointmentsQuery filters, CancellationToken cancellationToken)
    {
        var query = _context.DoctorAppointments
            .AsNoTracking()
            .Where(a => a.DoctorId == doctorId);

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
            .Select(a => new DoctorAppointmentResponse
            (
                a.Id,
                a.Patient.User.Name,
                a.DoctorSlot.Date,
                a.DoctorSlot.StartTime,
                a.DoctorSlot.EndTime,
                a.Status,
                a.AppointmentType,
                a.Address,

                // i didnt check by the data of the appointment cuz i want to send this data only if it ask me in the filters to get the bending home visit 
                // (for the pending home visit requests page so i dont make another endpoint 
                isPendingHomeVisits ? a.Patient.User.PhoneNumber : null,
                isPendingHomeVisits ? a.Notes : null,
                isPendingHomeVisits ? a.Patient.User.Gender : null
            ))
            .ToPagedListAsync(filters.Page, filters.PageSize, cancellationToken);
    }
}
