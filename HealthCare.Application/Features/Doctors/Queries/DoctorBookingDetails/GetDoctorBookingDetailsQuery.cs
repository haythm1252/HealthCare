using HealthCare.Application.Common.Result;
using HealthCare.Application.Features.Doctors.Contracts;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.Doctors.Queries.DoctorBookingDetails;

public record GetDoctorBookingDetailsQuery(Guid Id) : IRequest<Result<DoctorBookingDetailsResponse>>;
