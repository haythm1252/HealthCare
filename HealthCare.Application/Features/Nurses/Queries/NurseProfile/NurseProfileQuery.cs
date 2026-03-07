using HealthCare.Application.Common.Result;
using HealthCare.Application.Features.Nurses.Contracts;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.Nurses.Queries.NurseProfile
{
    public record NurseProfileQuery(string UserId) : IRequest<Result<NurseProfileResponse>>;
    
}
