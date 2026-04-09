using HealthCare.Application.Features.Auth.Contracts;
using HealthCare.Application.Features.DoctorAppointments.Contracts;
using HealthCare.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.Patients.MedicalRecordContracts;

public record MedicalRecordResponse(
    string Name,
    DateOnly DateOfBirth,
    Gender? Gender,
    decimal? Weight,
    DiseasesDto MedicalConditions,
    IEnumerable<DiagnosisDto> Diagnoses,
    IEnumerable<LabAppointmentTestResultsDto> LabResults,
    IEnumerable<RequiredTestDto> PendingRequiredTests
);
