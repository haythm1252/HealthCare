using System.Threading;
using System.Threading.Tasks;
using MediatR;
using HealthCare.Application.Common.Result;
using HealthCare.Application.Errors;
using HealthCare.Application.Features.Nurse.Contracts;
using HealthCare.Application.Interfaces.Repositories.UnitOfWork;
using Mapster;

namespace HealthCare.Application.Features.Nurse.Quries.NurseProfile
{
    internal class NurseProfileQueryHandler : IRequestHandler<NurseProfleQuery, Result<NurseProfileResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public NurseProfileQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<NurseProfileResponse>> Handle(NurseProfleQuery request, CancellationToken cancellationToken)
        {
            // Ensure `NurseProfleQuery` has a property named UserId (or change this to the correct name).
            // CS1061 occurs because the property used here doesn't exist on the request type.
            if (request is null)
                return Result.Failure<NurseProfileResponse>(UserErrors.NotFound);

            var isUserExist = await _unitOfWork.Nurses.AnyAsync(n => n.UserId == request.UserID, cancellationToken);
            if (!isUserExist)
            {
                return Result.Failure<NurseProfileResponse>(UserErrors.NotFound);
            }

            var nurse = await _unitOfWork.Nurses.GetAsync(n => n.UserId == request.UserID, asNoTracking: true, cancellationToken: cancellationToken);

            if (nurse == null)
            {
                return Result.Failure<NurseProfileResponse>(UserErrors.NotFound);
            }

            var nurseProfileResponse = nurse.Adapt<NurseProfileResponse>();
            return Result.Success(nurseProfileResponse);
        }
    }
}
