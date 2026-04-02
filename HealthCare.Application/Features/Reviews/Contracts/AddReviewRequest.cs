using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.Reviews.Contracts;

public record AddReviewRequest(
    Guid TargetId,
    string TargetType,
    int Rating,
    string? Comment
);