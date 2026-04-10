using HealthCare.Application.Common.Result;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.Specialities.Commands.UpdateSpeciality;

public record UpdateSpecialityCommand(
    Guid Id,
    string Name
) : IRequest<Result>;
