using HealthCare.Application.Common.Consts;
using HealthCare.Application.Common.Result;
using HealthCare.Application.Errors;
using HealthCare.Application.Features.Auth.Contracts;
using HealthCare.Application.Interfaces.Repositories.UnitOfWork;
using HealthCare.Domain.Users;
using Mapster;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace HealthCare.Application.Features.Users.Commands.MedicalStaffRegister;

public class MedicalStaffRegisterCommandHandler(IAuthService authService, IUnitOfWork unitOfWork) : IRequestHandler<MedicalStaffRegisterCommand, Result<RegisterResponse>>
{
    private readonly IAuthService _authService = authService;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result<RegisterResponse>> Handle(MedicalStaffRegisterCommand request, CancellationToken cancellationToken)
    {
        if (await _authService.IsUserExist(request.Email, cancellationToken))
            return Result.Failure<RegisterResponse>(UserErrors.DublicatedEmail);

        var result = await _authService.RegisterMedicalStaffAsync(request, cancellationToken);
        if (result.IsFailure)
            return result;

        // create domain record based on role
        if (request.Role == DefaultRoles.Doctor)
        {
            var doctor = new Doctor { UserId = result.Value.UserId };

            if (!await _unitOfWork.Specialties.AnyAsync(s => s.Id == request.SpecialityId, cancellationToken))
               return Result.Failure<RegisterResponse>(SpecialtyErrors.NotFound);

            doctor.SpecialtyId = (Guid) request.SpecialityId!;

            await _unitOfWork.Doctors.AddAsync(doctor, cancellationToken);
        }
        else if (request.Role == DefaultRoles.Nurse)
        {
            var nurse = new Nurse { UserId = result.Value.UserId };
            await _unitOfWork.Nurses.AddAsync(nurse, cancellationToken);
        }
        else if (request.Role == DefaultRoles.Lab)
        {
            var lab = new Lab { UserId = result.Value.UserId };
            await _unitOfWork.Labs.AddAsync(lab, cancellationToken);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return result;
    }
}
