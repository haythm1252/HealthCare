using HealthCare.Application.Features.Appointments.Contracts;
using HealthCare.Application.Interfaces.Repositories.Base;
using HealthCare.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Interfaces.Repositories;

public interface IDoctorAppointmentRepository : IBaseRepository<DoctorAppointment>
{
    Task<IEnumerable<AppointmentDto>> GetPatientAppointmentsAsync(Guid patientId, CancellationToken cancellationToken);
}
