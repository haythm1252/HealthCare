using HealthCare.Application.Common.Result;
using HealthCare.Application.Features.Nurses.Contracts;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.Nurses.Queries.GetShcedule;

public record NurseScheduleQuery(string UserId) : IRequest<Result<NurseScheduleResponse>>;
