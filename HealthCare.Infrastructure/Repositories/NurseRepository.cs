using HealthCare.Application.Common.Consts;
using HealthCare.Application.Common.Pagination;
using HealthCare.Application.Features.Labs.Contracts;
using HealthCare.Application.Features.Nurses.Contracts;
using HealthCare.Application.Features.Nurses.Queries.GetNurses;
using HealthCare.Application.Interfaces.Repositories;
using HealthCare.Domain.Users;
using HealthCare.Infrastructure.Persistence;
using HealthCare.Infrastructure.Repositories.Base;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Infrastructure.Repositories;

public class NurseRepository(ApplicationDbContext context) : BaseRepository<Nurse>(context), INurseRepository
{
    private readonly ApplicationDbContext _context = context;

    public async Task<PagedList<NurseResponse>> GetNursesWithFiltersAsync(GetNursesQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Nurses
            .AsNoTracking()
            .Where(n => !n.User.IsDisabled);

        if (!string.IsNullOrWhiteSpace(request.Search))
            query = query.Where(n => n.User.Name.Contains(request.Search));

        if (!string.IsNullOrWhiteSpace(request.City))
            query = query.Where(n => n.User.City.Contains(request.City));

        if (request.MinRate.HasValue)
            query = query.Where(n => n.Rating >= request.MinRate.Value);

        if (!string.IsNullOrEmpty(request.Sort))
            query = request.Sort switch
            {
                FiltersOptions.PriceAsc => query.OrderBy(n => n.HomeVisitFee),
                FiltersOptions.PriceDesc => query.OrderByDescending(n => n.HomeVisitFee),
                FiltersOptions.RateAsc => query.OrderBy(n => n.Rating),
                FiltersOptions.RateDesc => query.OrderByDescending(n => n.Rating),
                _ => query
            };

        return await query
            .Select(n => new NurseResponse(
                n.Id,
                n.User.Name,
                n.User.City,
                n.HomeVisitFee,
                n.HourPrice,
                n.Rating,
                n.RatingsCount,
                n.ProfilePictureUrl
            )).ToPagedListAsync(request.Page, request.PageSize, cancellationToken);
    }
}
