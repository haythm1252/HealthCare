using HealthCare.Application.Common.Result;
using HealthCare.Application.Features.Reviews.Contracts;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.Reviews.Commands.AddReview;

public record AddReviewCommand(
    string UserId,
    Guid TargetId,
    string TargetType,
    int Rating,
    string? Comment
) : IRequest<Result<ReviewResponse>>;
