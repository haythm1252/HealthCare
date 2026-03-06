using HealthCare.Application.Common.Result;
using HealthCare.Application.Features.Patients.Contracts;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.Patients.Queries.PatientProfile;

public record PatientProfileQuery(string UserId) : IRequest<Result<PatientProfileResponse>>;
