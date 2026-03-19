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

        if (request.TestId.HasValue)
            query = query.Where(l => l.LabTests.Any(lt => lt.TestId == request.TestId.Value));

        if(!string.IsNullOrWhiteSpace(request.Search))
            query = query.Where(l => l.User.Name.Contains(request.Search));

        if(!string.IsNullOrWhiteSpace(request.City))
            query = query.Where(l => l.User.City.Contains(request.City));

        if (request.MinRate.HasValue)
            query = query.Where(l => l.Rating >= request.MinRate.Value);

        if (!string.IsNullOrEmpty(request.Sort))
            query = request.Sort switch
            { 
                FiltersOptions.RateAsc => query.OrderBy(d => d.Rating),
                FiltersOptions.RateDesc => query.OrderByDescending(d => d.Rating),
                _ => query
            };

        return await query
            .Select(l => new LabResponse(
                l.Id,
                l.User.Name,
                l.User.Address,
                l.Rating,
                l.RatingsCount,
                l.ProfilePictureUrl
            )).ToPagedListAsync(request.Page, request.PageSize, cancellationToken);
    }
}
