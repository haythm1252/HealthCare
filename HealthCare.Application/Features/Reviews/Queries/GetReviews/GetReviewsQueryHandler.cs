using HealthCare.Application.Common.Pagination;
using HealthCare.Application.Common.Result;
using HealthCare.Application.Errors;
using HealthCare.Application.Features.Reviews.Contracts;
using HealthCare.Application.Interfaces.Repositories.UnitOfWork;
using HealthCare.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.Reviews.Queries.GetReviews;

public class GetReviewsQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetReviewsQuery, Result<PagedList<ReviewResponse>>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result<PagedList<ReviewResponse>>> Handle(GetReviewsQuery request, CancellationToken cancellationToken)
    {
        var targetType = Enum.Parse<TargetType>(request.TargetType,true);

        if (!await TargetExistsAsync(request.TargetId, targetType, cancellationToken))
            return Result.Failure<PagedList<ReviewResponse>>(ReviewErrors.TargetNotFound);

        var query = _unitOfWork.Reviews.AsQueryable()
            .AsNoTracking()
            .Include(r => r.Patient)
                .ThenInclude(p => p.User)
            .Where(r => r.TargetId == request.TargetId && r.TargetType == targetType);

        if (request.SortByRating)
            query = query.OrderByDescending(r => r.Rating)
                .ThenByDescending(r => r.LastModified ?? r.CreatedAt);
        else if (request.SortByDate)
            query = query.OrderByDescending(r => r.LastModified ?? r.CreatedAt);

        var reviews = await query.Select(r => new ReviewResponse
            (
                r.Id,
                r.Patient.User.Name,
                r.Rating,
                r.Comment,
                r.LastModified ?? r.CreatedAt,
                r.LastModified.HasValue && r.LastModified > r.CreatedAt
            )).ToPagedListAsync(request.Page, request.PageSize, cancellationToken);

        return Result.Success(reviews);
    }




    private async Task<bool> TargetExistsAsync(Guid targetId, TargetType type, CancellationToken cancellationToken)
    {
        return type switch
        {
            TargetType.Doctor => await _unitOfWork.Doctors.AsQueryable().AnyAsync(d => d.Id == targetId, cancellationToken),
            TargetType.Nurse => await _unitOfWork.Nurses.AsQueryable().AnyAsync(n => n.Id == targetId, cancellationToken), 
            TargetType.Lab => await _unitOfWork.Labs.AsQueryable().AnyAsync(l => l.Id == targetId, cancellationToken),
            _ => false
        };
    }
}
