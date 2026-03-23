using HealthCare.Application.Common.Result;
using HealthCare.Application.Features.Doctors.Contracts;
using HealthCare.Application.Features.Nurses.Contracts;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.Nurses.Queries.NurseBookingDetails;

public record GetNurseBookingDetailsQuery(Guid Id) : IRequest<Result<NurseBookingDetailsResponse>>;