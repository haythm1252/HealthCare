using HealthCare.Application.Common.Consts;
using HealthCare.Application.Common.Pagination;
using HealthCare.Application.Features.Labs.Contracts;
using HealthCare.Application.Features.Labs.Queries.GetLabs;
using HealthCare.Application.Interfaces.Repositories;
using HealthCare.Domain.Users;
using HealthCare.Infrastructure.Persistence;
using HealthCare.Infrastructure.Repositories.Base;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Infrastructure.Repositories;

public class LabRepository(ApplicationDbContext context) : BaseRepository<Lab>(context), ILabRepository
{
    private readonly ApplicationDbContext _context = context;

    public async Task<PagedList<LabResponse>> GetLabsWithFiltersAsync(GetLabsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Labs
            .AsNoTracking()
            .Where(l => !l.User.IsDisabled);

        var hasTests = request.TestIds?.Any() == true;
        if (hasTests)
            query = query.Where(l => l.LabTests.Any(lt => request.TestIds!.Contains(lt.TestId) && !lt.IsDeleted));

        if (!string.IsNullOrWhiteSpace(request.Search))
            query = query.Where(l => l.User.Name.Contains(request.Search));

        if(!string.IsNullOrWhiteSpace(request.City))
            query = query.Where(l => l.User.City.Contains(request.City));

        if (request.MinRate.HasValue)
            query = query.Where(l => l.Rating >= request.MinRate.Value);

        if(hasTests)
            query = query.OrderByDescending(l => l.LabTests.Count(lt => request.TestIds!.Contains(lt.TestId) && !lt.IsDeleted));

        else if (!string.IsNullOrEmpty(request.Sort))
            query = request.Sort == FiltersOptions.RateAsc
                ? query.OrderBy(l => l.Rating)
                : query.OrderByDescending(l => l.Rating);

        return await query
            .Select(l => new LabResponse(
                l.Id,
                l.User.Name,
                l.User.Address,
                l.Rating,
                l.RatingsCount,
                l.ProfilePictureUrl,

                request.TestIds != null ? l.LabTests.Count(lt => request.TestIds.Contains(lt.TestId) && !lt.IsDeleted) : null,
                request.TestIds != null ? request.TestIds.Count() : null,
                request.TestIds != null ? l.LabTests.Where(lt => request.TestIds.Contains(lt.TestId) && !lt.IsDeleted)
                                            .Select(lt => lt.Test.Name).ToList() : null
            ))
            .ToPagedListAsync(request.Page, request.PageSize, cancellationToken);
    }
}
