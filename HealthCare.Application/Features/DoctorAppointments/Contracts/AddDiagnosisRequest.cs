using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.DoctorAppointments.Contracts;

public record AddDiagnosisRequest(
    string Diagnosis,
    string Prescriptions,
    IEnumerable<Guid>? RequiredTests
);
