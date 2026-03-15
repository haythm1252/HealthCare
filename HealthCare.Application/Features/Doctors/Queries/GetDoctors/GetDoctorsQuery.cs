using HealthCare.Application.Common.Pagination;
using HealthCare.Application.Features.Doctors.Contracts;
using HealthCare.Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.Doctors.Queries.GetDoctors;

public record GetDoctorsQuery(
    Guid? SpecialityId,
    string? Search,
    string? City,
    decimal? MinRate,
    string? Sort,
    string? AppointmentType,
    int Page = 1,
    int PageSize = 20
) : IRequest<PagedList<DoctorResponse>>;
