using HealthCare.Application.Common.Pagination;
using HealthCare.Application.Common.Result;
using HealthCare.Application.Errors;
using HealthCare.Application.Features.Community.Contracts;
using HealthCare.Application.Interfaces.Repositories.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HealthCare.Application.Features.Community.Queries.GetDoctorPosts;

public class GetDoctorPostsQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetDoctorPostsQuery, Result<PagedList<PostResponse>>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result<PagedList<PostResponse>>> Handle(GetDoctorPostsQuery request, CancellationToken cancellationToken)
    {
        var doctor = await _unitOfWork.Doctors.AsQueryable()
            .Where(d => d.UserId == request.UserId)
            .Select(d => new {d.Id})
            .FirstOrDefaultAsync(cancellationToken);

        if (doctor is null)
            return Result.Failure<PagedList<PostResponse>>(DoctorErrors.NotFound);

        var posts = await _unitOfWork.Posts.AsQueryable()
            .Where(p => p.DoctorId == doctor.Id)
            .Select(p => new PostResponse(
                p.Id,
                p.Title,
                p.Content,
                p.AttachmentUrl,
                p.IsPublished,
                p.IsContainsMedia,
                p.Doctor.Id,
                null,
                null,
                p.SpecialtyId,
                p.Specialty.Name,
                p.CreatedAt
            ))
            .ToPagedListAsync(request.Page, request.PageSize, cancellationToken);

        return Result.Success(posts);
    }
}
