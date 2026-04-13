using HealthCare.Application.Common.Pagination;
using HealthCare.Application.Common.Result;
using HealthCare.Application.Features.Community.Contracts;
using MediatR;

namespace HealthCare.Application.Features.Community.Queries.GetPosts;

public record GetPostsQuery(
    string UserRole,
    string? Search,
    Guid? SpecialtyId,
    bool? PendingPosts,
    int Page,
    int PageSize
) : IRequest<Result<PagedList<PostResponse>>>;
