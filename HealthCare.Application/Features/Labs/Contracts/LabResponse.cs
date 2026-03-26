using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.Labs.Contracts;

public record LabResponse
(
    Guid Id,
    string Name,
    string Address,
    decimal Rating,
    int RatingsCount,
    string ProfilePictureUrl,

    int? MatchedTestsCount = null,
    int? TotalRequestedTests = null,
    List<string>? MatchedTestsNames = null
);
