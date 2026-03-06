using HealthCare.Application.Common.Result;
using HealthCare.Application.Features.Nurse.Contracts;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.Nurse.Quries.NurseProfile
{
    public record NurseProfleQuery(String UserID) : IRequest<Result<NurseProfileResponse>>;
    
}
