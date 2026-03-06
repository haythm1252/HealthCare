using HealthCare.Application.Features.Patients.Contracts;
using HealthCare.Application.Features.Patients.Queries.PatientProfile;
using HealthCare.Application.Interfaces.Repositories.Base;
using HealthCare.Domain.Users;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Interfaces.Repositories;

public interface IPatientRepository : IBaseRepository<Patient>
{
    Task<PatientProfileResponse?> GetPatientProfileAsync(string userId, CancellationToken cancellationToken = default);
}
