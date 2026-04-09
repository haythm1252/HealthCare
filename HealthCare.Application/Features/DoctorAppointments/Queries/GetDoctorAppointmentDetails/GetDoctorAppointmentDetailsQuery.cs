using HealthCare.Application.Common.Result;
using HealthCare.Application.Features.DoctorAppointments.Contracts;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.DoctorAppointments.Queries.GetDoctorAppointmentDetails;

public record GetDoctorAppointmentDetailsQuery(
    string UserId,
    string UserRole,
    Guid AppointmentId
) : IRequest<Result<DoctorAppointmentDetailsResponse>>;