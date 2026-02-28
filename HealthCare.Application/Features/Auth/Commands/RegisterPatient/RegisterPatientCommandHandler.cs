using HealthCare.Application.Common.Result;
using HealthCare.Application.Errors;
using HealthCare.Application.Features.Auth.Contracts;
using HealthCare.Application.Interfaces.Repositories.UnitOfWork;
using HealthCare.Domain.Users;
using Mapster;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.Auth.Commands.RegisterPatient;

public class RegisterPatientCommandHandler(IAuthService authService, IUnitOfWork unitOfWork) : IRequestHandler<RegisterPatientCommand, Result<RegisterResponse>>
{
    private readonly IAuthService _authService = authService;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result<RegisterResponse>> Handle(RegisterPatientCommand request, CancellationToken cancellationToken)
    {       

        if(await _authService.IsUserExist(request.Email, cancellationToken))
            return Result.Failure<RegisterResponse>(UserErrors.DublicatedEmail);

        var result = await _authService.RegisterPatientAsync(request, cancellationToken);
        if (result.IsFailure)
            return result;

        var patient = request.Adapt<Patient>();
        patient.UserId = result.Value.UserId;
        await _unitOfWork.Patients.AddAsync(patient, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);  

        return result;
    }

}
