using HealthCare.Application.Features.Patients.Contracts;
using HealthCare.Application.Features.Patients.Queries.PatientProfile;
using HealthCare.Application.Interfaces.Repositories;
using HealthCare.Domain.Users;
using HealthCare.Infrastructure.Persistence;
using HealthCare.Infrastructure.Repositories.Base;
using Mapster;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Infrastructure.Repositories;

public class PatientRepository(ApplicationDbContext context) : BaseRepository<Patient>(context), IPatientRepository
{
}
