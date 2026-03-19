using HealthCare.Application.Common.Result;
using HealthCare.Application.Features.LabTest.Contracts;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.LabTest.Queries.GetLabTests;

public record GetLabTestsQuery(string UserId) : IRequest<Result<IEnumerable<LabTestResponse>>>;
