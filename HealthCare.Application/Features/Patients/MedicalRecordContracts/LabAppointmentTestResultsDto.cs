using HealthCare.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.Patients.MedicalRecordContracts;

public record LabAppointmentTestResultsDto(
    Guid AppointmentId,
    string LabName,
    DateOnly AppointmentDate,
    AppointmentType AppointmentType,
    IEnumerable<TestResultDto> Results

);
