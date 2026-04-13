using HealthCare.Application.Common.Pagination;
using HealthCare.Application.Common.Result;
using HealthCare.Application.Features.Community.Contracts;
using MediatR;

namespace HealthCare.Application.Features.Community.Queries.GetDoctorPosts;

public record GetDoctorPostsQuery(
    string UserId,
    int Page = 1,
    int PageSize = 20
) : IRequest<Result<PagedList<PostResponse>>>;
