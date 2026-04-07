using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.Appointments.Contracts;

public record PatientAppointmentHistoryResponse(
    IEnumerable<AppointmentDto> DoctorAppointments,
    IEnumerable<AppointmentDto> NurseAppointments,
    IEnumerable<AppointmentDto> LabAppointments
);
