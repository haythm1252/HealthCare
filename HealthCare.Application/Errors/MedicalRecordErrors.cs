using HealthCare.Application.Common.Result;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Errors;

public static class MedicalRecordErrors
{
    public static readonly Error MissingPatientId = 
        new("MedicalRecord.MissingId","A specific Patient ID must be provided when a healthcare provider accesses medical records.",400);

    public static readonly Error PatientNotFound =
        new("MedicalRecord.PatientNotFound", "The Patient with the provided id is not found", 404);

    public static readonly Error AccessDenied = 
        new("MedicalRecord.AccessDenied", "You do not have a registered appointment with this patient. Access to medical history is restricted.", 403);
}
