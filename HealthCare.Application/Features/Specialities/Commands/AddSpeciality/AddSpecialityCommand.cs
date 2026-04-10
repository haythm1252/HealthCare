using HealthCare.Application.Common.Result;
using HealthCare.Application.Features.Specialities.Contracts;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.Specialities.Commands.AddSpeciality;

public record AddSpecialityCommand(
   string Name
) : IRequest<Result<SpecialityResponse>>;
