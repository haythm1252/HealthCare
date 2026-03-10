using HealthCare.Application.Features.Specialities.Contracts;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.Specialities.Queries;

public record GetSpecialitiesQuery : IRequest<IEnumerable<SpecialityResponse>>;
