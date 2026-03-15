using HealthCare.Application.Common.Pagination;
using HealthCare.Application.Features.Doctors.Contracts;
using HealthCare.Application.Features.Doctors.Queries.GetDoctors;
using HealthCare.Application.Interfaces.Repositories.Base;
using HealthCare.Domain.Users;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Interfaces.Repositories;

public interface IDoctorRepository : IBaseRepository<Doctor>
{
    Task<PagedList<DoctorResponse>> GetDoctorsWithFiltersAsync(GetDoctorsQuery request, CancellationToken cancellationToken);
}
