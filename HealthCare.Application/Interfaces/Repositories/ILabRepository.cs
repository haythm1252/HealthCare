using HealthCare.Application.Common.Pagination;
using HealthCare.Application.Features.Labs.Contracts;
using HealthCare.Application.Features.Labs.Queries.GetLabs;
using HealthCare.Application.Interfaces.Repositories.Base;
using HealthCare.Domain.Users;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Interfaces.Repositories;

public interface ILabRepository : IBaseRepository<Lab>
{
    Task<PagedList<LabResponse>> GetLabsWithFiltersAsync(GetLabsQuery request, CancellationToken cancellationToken);
}
