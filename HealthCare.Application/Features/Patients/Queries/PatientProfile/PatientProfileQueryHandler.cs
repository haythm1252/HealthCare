using HealthCare.Application.Common.Result;
using HealthCare.Application.Errors;
using HealthCare.Application.Features.Patients.Contracts;
using HealthCare.Application.Interfaces.Repositories.UnitOfWork;
using HealthCare.Domain.Users;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
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
        var patient = await _unitOfWork.Patients.AsQueryable()
            .Where(p => p.UserId == request.UserId)
            .Include(p => p.User)
            .AsNoTracking()
            .SingleOrDefaultAsync(cancellationToken);

        if (patient == null)
            return Result.Failure<PatientProfileResponse>(UserErrors.NotFound);

        var patientProfileResponse = patient.Adapt<PatientProfileResponse>();
        return Result.Success(patientProfileResponse);
    }
}
