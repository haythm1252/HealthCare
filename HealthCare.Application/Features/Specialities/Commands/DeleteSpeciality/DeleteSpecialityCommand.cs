using HealthCare.Application.Common.Result;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.Specialities.Commands.DeleteSpeciality;

public record DeleteSpecialityCommand(
    Guid Id
) : IRequest<Result>;
