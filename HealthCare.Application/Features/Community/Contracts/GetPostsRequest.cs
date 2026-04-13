using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.Community.Contracts;

public record GetPostsRequest(
    string? Search = null,
    Guid? SpecialtyId = null,
    bool? PendingPosts = null,
    int? Page = 1,
    int? PageSize = 10
);
