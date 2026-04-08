using HealthCare.Application.Common.Result;
using HealthCare.Application.Features.Patients.MedicalRecordContracts;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.Patients.Queries.GetPatientMedicalRecord;

public record GetPatientMedicalRecordQuery(
    string UserId,
    string UserRole,
    Guid? PatientId = null
) : IRequest<Result<MedicalRecordResponse>>;
