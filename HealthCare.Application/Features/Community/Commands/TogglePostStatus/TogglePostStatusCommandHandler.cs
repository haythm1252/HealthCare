using HealthCare.Application.Common.Result;
using HealthCare.Application.Errors;
using HealthCare.Application.Interfaces.Repositories.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HealthCare.Application.Features.Community.Commands.TogglePostStatus;

public class TogglePostStatusCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<TogglePostStatusCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result> Handle(TogglePostStatusCommand request, CancellationToken cancellationToken)
    {
        var updatedCount = await _unitOfWork.Posts.AsQueryable()
            .Where(p => p.Id == request.PostId)
            .ExecuteUpdateAsync(setters => setters
                    .SetProperty(p => p.IsPublished, p => !p.IsPublished)
                    .SetProperty(p => p.LastModified, DateTime.UtcNow),
                cancellationToken
            );

        if (updatedCount <= 0)
            return Result.Failure(PostErrors.NotFound);

        return Result.Success();
    }
}
