using HealthCare.Application.Common.Result;
using HealthCare.Application.Features.Labs.Contracts;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.Labs.Queries.LabProfile
{
    public record LabProfileQuery(string UserId) : IRequest<Result<LabProfileResponse>>;
}
