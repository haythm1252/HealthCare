using HealthCare.Application.Common.Pagination;
using HealthCare.Application.Features.Nurses.Contracts;
using HealthCare.Application.Features.Nurses.Queries.GetNurses;
using HealthCare.Application.Interfaces.Repositories.Base;
using HealthCare.Domain.Users;

namespace HealthCare.Application.Interfaces.Repositories;

public interface INurseRepository : IBaseRepository<Nurse>
{
    Task<PagedList<NurseResponse>> GetNursesWithFiltersAsync(GetNursesQuery request, CancellationToken cancellationToken);
}
