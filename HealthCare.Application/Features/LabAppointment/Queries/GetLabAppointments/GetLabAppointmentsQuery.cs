using HealthCare.Application.Common.Pagination;
using HealthCare.Application.Common.Result;
using HealthCare.Application.Features.DoctorAppointments.Contracts;
using HealthCare.Application.Features.LabAppointment.Contracts;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.LabAppointment.Queries.GetLabAppointments;

public record GetLabAppointmentsQuery(
    string UserId,
    string? Search,
    string? Status,
    string? AppointmentType,
    int Page = 1,
    int PageSize = 20
) : IRequest<Result<PagedList<LabAppointmentResponse>>>;
