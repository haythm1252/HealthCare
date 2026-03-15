using HealthCare.Application.Common.Consts;
using HealthCare.Application.Common.Pagination;
using HealthCare.Application.Features.Doctors.Contracts;
using HealthCare.Application.Features.Doctors.Queries.GetDoctors;
using HealthCare.Application.Interfaces.Repositories;
using HealthCare.Domain.Users;
using HealthCare.Infrastructure.Persistence;
using HealthCare.Infrastructure.Repositories.Base;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Infrastructure.Repositories;

public class DoctorRepository(ApplicationDbContext context) : BaseRepository<Doctor>(context), IDoctorRepository
{
    private readonly ApplicationDbContext _context = context;

    public async Task<PagedList<DoctorResponse>> GetDoctorsWithFiltersAsync(GetDoctorsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Doctors
            .AsNoTracking()
            .AsQueryable();

        if (request.SpecialityId.HasValue)
            query = query.Where(d => d.SpecialtyId == request.SpecialityId.Value);

        if(!string.IsNullOrWhiteSpace(request.Search))
            query = query.Where(d => d.User.Name.Contains(request.Search));

        if (!string.IsNullOrEmpty(request.City))
            query = query.Where(l => EF.Functions.Like(l.User.City, $"%{request.City}%"));

        if (!string.IsNullOrEmpty(request.AppointmentType))
            switch (request.AppointmentType)
            {
                case FiltersOptions.Home:
                    query = query.Where(d => d.AllowHomeVisit);
                    break;
                case FiltersOptions.Online:
                    query = query.Where(d => d.AllowOnlineConsultation);
                    break;

            }

        if (request.MinRate.HasValue)
            query = query.Where(d => d.Rating >= request.MinRate.Value);


        if (!string.IsNullOrEmpty(request.Sort))
            query = request.Sort switch
            {
                FiltersOptions.PriceAsc => 
                    query.OrderBy(d => request.AppointmentType == FiltersOptions.Home ? d.HomeFee : 
                    request.AppointmentType == FiltersOptions.Online ? d.OnlineFee : d.ClinicFee),

                FiltersOptions.PriceDesc => 
                    query.OrderByDescending(d => request.AppointmentType == FiltersOptions.Home ? d.HomeFee :
                    request.AppointmentType == FiltersOptions.Online ? d.OnlineFee : d.ClinicFee),

                FiltersOptions.RateAsc => query.OrderBy(d => d.Rating),
                FiltersOptions.RateDesc => query.OrderByDescending(d => d.Rating),
                _ => query
            };


        return await query
            .Select(d => new DoctorResponse(
                d.Id,
                d.User.Name,
                d.Specialty.Name,
                d.Title,
                d.User.Address,
                Fee:  request.AppointmentType == FiltersOptions.Home ? d.HomeFee :
                      request.AppointmentType == FiltersOptions.Online ? d.OnlineFee : d.ClinicFee,
                d.Rating,
                d.RatingsCount,
                d.AllowHomeVisit,
                d.AllowOnlineConsultation,
                d.ProfilePictureUrl
            ))
            .ToPagedListAsync(request.Page, request.PageSize,cancellationToken);
    }
}
