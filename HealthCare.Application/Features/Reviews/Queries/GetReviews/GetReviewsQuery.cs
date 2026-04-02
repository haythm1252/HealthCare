using HealthCare.Application.Common.Pagination;
using HealthCare.Application.Common.Result;
using HealthCare.Application.Features.Reviews.Contracts;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.Reviews.Queries.GetReviews;

public record GetReviewsQuery(
    Guid TargetId,
    string TargetType,
    bool SortByDate = false,
    bool SortByRating = false,
    int Page = 1,
    int PageSize = 20
) : IRequest<Result<PagedList<ReviewResponse>>>;
