using HealthCare.Application.Common.Pagination;
using HealthCare.Application.Features.Appointments.Contracts;
using HealthCare.Application.Features.LabAppointment.Contracts;
using HealthCare.Application.Features.LabAppointment.Queries.GetLabAppointments;
using HealthCare.Application.Interfaces.Repositories.Base;
using HealthCare.Domain.Entities;

namespace HealthCare.Application.Interfaces.Repositories;

public interface ILabAppointmentRepository : IBaseRepository<LabAppointment>
{
    Task<IEnumerable<AppointmentDto>> GetPatientAppointmentsAsync(Guid patientId, CancellationToken cancellationToken);

    Task<PagedList<LabAppointmentResponse>> GetLabAppointmentsWithFiltersAsync
        (Guid labId, GetLabAppointmentsQuery filters, CancellationToken cancellationToken);

}
