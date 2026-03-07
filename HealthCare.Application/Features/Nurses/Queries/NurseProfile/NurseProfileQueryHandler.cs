using System.Threading;
using System.Threading.Tasks;
using MediatR;
using HealthCare.Application.Common.Result;
using HealthCare.Application.Errors;
using HealthCare.Application.Interfaces.Repositories.UnitOfWork;
using Mapster;
using HealthCare.Application.Features.Nurses.Contracts;
using Microsoft.EntityFrameworkCore;

namespace HealthCare.Application.Features.Nurses.Queries.NurseProfile
{
    internal class NurseProfileQueryHandler : IRequestHandler<NurseProfileQuery, Result<NurseProfileResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public NurseProfileQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<NurseProfileResponse>> Handle(NurseProfileQuery request, CancellationToken cancellationToken)
        {
            var nurse = await _unitOfWork.Nurses.AsQueryable()
                .Where(n => n.UserId == request.UserId)
                .Include(n => n.User)
                .AsNoTracking()
                .SingleOrDefaultAsync(cancellationToken);

            if (nurse == null)
            {
                return Result.Failure<NurseProfileResponse>(UserErrors.NotFound);
            }

            var nurseProfileResponse = nurse.Adapt<NurseProfileResponse>();
            return Result.Success(nurseProfileResponse);
        }
    }
}
