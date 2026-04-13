using HealthCare.Application.Common.Consts;
using HealthCare.Application.Common.Pagination;
using HealthCare.Application.Common.Result;
using HealthCare.Application.Errors;
using HealthCare.Application.Features.Community.Contracts;
using HealthCare.Application.Interfaces.Repositories.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HealthCare.Application.Features.Community.Queries.GetPosts;

public class GetPostsQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetPostsQuery, Result<PagedList<PostResponse>>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result<PagedList<PostResponse>>> Handle(GetPostsQuery request, CancellationToken cancellationToken)
    {
        // only admin can request pending posts
        if (request.PendingPosts == true && request.UserRole != DefaultRoles.Admin)
            return Result.Failure<PagedList<PostResponse>>(PostErrors.Unauthorized);

        var query = _unitOfWork.Posts.AsQueryable();

        if (request.PendingPosts == true)
            query = query.Where(p => !p.IsPublished);
        else
            query = query.Where(p => p.IsPublished);


        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var searchTerm = request.Search.ToLower();
            query = query.Where(p => 
                p.Title.ToLower().Contains(searchTerm) || 
                p.Content.ToLower().Contains(searchTerm)
            );
        }

        if (request.SpecialtyId.HasValue && request.SpecialtyId != Guid.Empty)
            query = query.Where(p => p.SpecialtyId == request.SpecialtyId);

        query = query.OrderByDescending(p =>
            request.UserRole == DefaultRoles.Admin ? p.CreatedAt : (p.LastModified ?? p.CreatedAt)
        );

        var posts = await query
            .Select(p => new PostResponse(
                p.Id,
                p.Title,
                p.Content,
                p.AttachmentUrl,
                p.IsPublished,
                p.IsContainsMedia,
                p.DoctorId,
                p.Doctor.User.Name,
                p.Doctor.ProfilePictureUrl,
                p.SpecialtyId,
                p.Specialty.Name,
                request.UserRole == DefaultRoles.Admin ? p.CreatedAt : (p.LastModified ?? p.CreatedAt)
            ))
            .ToPagedListAsync(request.Page, request.PageSize, cancellationToken);

        return Result.Success(posts);
    }
}
