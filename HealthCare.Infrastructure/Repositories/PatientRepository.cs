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
    private readonly ApplicationDbContext _context = context;

    public async Task<PatientProfileResponse?> GetPatientProfileAsync(string userId, CancellationToken cancellationToken = default)
    {
        return await _context.Patients.Where(p => p.UserId == userId)
                        .AsNoTracking()
                        .ProjectToType<PatientProfileResponse>()
                        .SingleOrDefaultAsync(cancellationToken);
    }
}
