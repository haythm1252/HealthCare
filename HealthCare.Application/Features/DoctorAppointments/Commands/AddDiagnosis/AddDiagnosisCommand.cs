using HealthCare.Application.Common.Result;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.DoctorAppointments.Commands.AddDiagnosis;

public record AddDiagnosisCommand(
    string UserId,
    Guid AppointmentId,
    string Diagnosis,
    string Prescriptions,
    IEnumerable<Guid>? RequiredTests
) : IRequest<Result>;
