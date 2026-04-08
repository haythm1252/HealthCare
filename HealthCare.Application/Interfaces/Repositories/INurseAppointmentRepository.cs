using HealthCare.Application.Common.Pagination;
using HealthCare.Application.Features.Appointments.Contracts;
using HealthCare.Application.Features.NurseAppointments.Contracts;
using HealthCare.Application.Features.NurseAppointments.Queries.GetNurseAppointments;
using HealthCare.Application.Interfaces.Repositories.Base;
using HealthCare.Domain.Entities;

namespace HealthCare.Application.Interfaces.Repositories;

public interface INurseAppointmentRepository : IBaseRepository<NurseAppointment>
{
    Task<IEnumerable<AppointmentDto>> GetPatientAppointmentsAsync(Guid patientId, CancellationToken cancellationToken);
    Task<PagedList<NurseAppointmentResponse>> GetNurseAppointmentsWithFiltersAsync
        (Guid nurseId, GetNurseAppointmentsQuery filters, CancellationToken cancellationToken);
}
