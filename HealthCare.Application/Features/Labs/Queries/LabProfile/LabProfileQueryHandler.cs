using HealthCare.Application.Common.Result;
using HealthCare.Application.Errors;
using HealthCare.Application.Features.Labs.Contracts;
using HealthCare.Application.Features.Nurses.Contracts;
using HealthCare.Application.Interfaces.Repositories.UnitOfWork;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.Labs.Queries.LabProfile
{
    public class LabProfileQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<LabProfileQuery, Result<LabProfileResponse>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<Result<LabProfileResponse>> Handle(LabProfileQuery request, CancellationToken cancellationToken)
        {
            var lab = await _unitOfWork.Labs.AsQueryable()
                .Where(n => n.UserId == request.UserId)
                .Include(n => n.User)
                .AsNoTracking()
                .SingleOrDefaultAsync(cancellationToken);

            if (lab == null)
                return Result.Failure<LabProfileResponse>(UserErrors.NotFound);


            var labProfile = lab.Adapt<LabProfileResponse>();
            return Result.Success(labProfile);
        }
    }
}
