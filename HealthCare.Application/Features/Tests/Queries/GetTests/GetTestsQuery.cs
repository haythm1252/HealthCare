using HealthCare.Application.Features.Tests.Contracts;
using MediatR;
using Microsoft.EntityFrameworkCore.Query.Internal;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.Tests.Queries.GetTests;

public record GetTestsQuery : IRequest<IEnumerable<TestResponse>>;
