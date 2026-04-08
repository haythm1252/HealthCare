using HealthCare.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.Patients.MedicalRecordContracts;

public record DiagnosisDto(
    Guid AppointmentId,
    string DoctorName,
    DateOnly AppointmentDate,
    AppointmentType AppointmentType,
    string? Diagnosis,
    string? Prescription,
    IEnumerable<RequiredTestDto> RequiredTests
);