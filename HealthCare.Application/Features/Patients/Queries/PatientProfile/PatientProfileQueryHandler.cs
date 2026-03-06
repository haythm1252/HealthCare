using HealthCare.Application.Common.Result;
using HealthCare.Application.Errors;
using HealthCare.Application.Features.Patients.Contracts;
using HealthCare.Application.Interfaces.Repositories.UnitOfWork;
using HealthCare.Domain.Users;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace HealthCare.Application.Features.Patients.Queries.PatientProfile;

public class PatientProfileQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<PatientProfileQuery, Result<PatientProfileResponse>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result<PatientProfileResponse>> Handle(PatientProfileQuery request, CancellationToken cancellationToken)
    {
        var isUserExist = await _unitOfWork.Patients.AnyAsync(p => p.UserId == request.UserId, cancellationToken);
        if (!isUserExist) 
            return Result.Failure<PatientProfileResponse>(UserErrors.NotFound);

        var patientProfileResponse = await _unitOfWork.Patients.GetPatientProfileAsync(request.UserId, cancellationToken);

        if (patientProfileResponse == null)
            return Result.Failure<PatientProfileResponse>(UserErrors.NotFound);

        return Result.Success(patientProfileResponse);
    }
}
