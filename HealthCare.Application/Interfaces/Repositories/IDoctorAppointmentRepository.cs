using HealthCare.Application.Common.Pagination;
using HealthCare.Application.Features.Appointments.Contracts;
using HealthCare.Application.Features.DoctorAppointments.Contracts;
using HealthCare.Application.Features.DoctorAppointments.Queries.GetDoctorAppointmentDetails;
using HealthCare.Application.Features.DoctorAppointments.Queries.GetDoctorAppointments;
using HealthCare.Application.Interfaces.Repositories.Base;
using HealthCare.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Interfaces.Repositories;

public interface IDoctorAppointmentRepository : IBaseRepository<DoctorAppointment>
{
    Task<IEnumerable<AppointmentDto>> GetPatientAppointmentsAsync(Guid patientId, CancellationToken cancellationToken);
    Task<PagedList<DoctorAppointmentResponse>> GetDoctorAppointmentsWithFiltersAsync
        (Guid doctorId, GetDoctorAppointmentsQuery filters, CancellationToken cancellationToken);
}
