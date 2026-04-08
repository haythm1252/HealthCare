using HealthCare.Application.Common.Consts;
using HealthCare.Application.Common.Result;
using HealthCare.Application.Errors;
using HealthCare.Application.Features.Patients.MedicalRecordContracts;
using HealthCare.Application.Interfaces.Repositories.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.Patients.Queries.GetPatientMedicalRecord;

public class GetPatientMedicalRecordQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetPatientMedicalRecordQuery, Result<MedicalRecordResponse>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result<MedicalRecordResponse>> Handle(GetPatientMedicalRecordQuery request, CancellationToken cancellationToken)
    {
        Guid patientId = Guid.Empty;

        if (request.UserRole == DefaultRoles.Patient)
        {
            patientId = await _unitOfWork.Patients.AsQueryable()
                .Where(p => p.UserId == request.UserId).Select(p => p.Id)
                .SingleOrDefaultAsync(cancellationToken);

            if (patientId == Guid.Empty)
                return Result.Failure<MedicalRecordResponse>(UserErrors.NotFound);
        }
        else 
        {
            if (!request.PatientId.HasValue)
                return Result.Failure<MedicalRecordResponse>(MedicalRecordErrors.MissingPatientId);

            patientId = request.PatientId.Value;

            bool hasAccess = await CheckProviderAccess(request.UserId, request.UserRole, patientId);
            if (!hasAccess) 
                return Result.Failure<MedicalRecordResponse>(MedicalRecordErrors.AccessDenied);
        }

        var medicalRecordResponse = await _unitOfWork.Patients.GetPatientMedicalRecordAsync(patientId, cancellationToken);

        if(medicalRecordResponse is null)
            return Result.Failure<MedicalRecordResponse>(MedicalRecordErrors.PatientNotFound);

        return Result.Success(medicalRecordResponse);
    }

    private async Task<bool> CheckProviderAccess(string userId, string role, Guid patientId)
    {
        return role switch
        {
            DefaultRoles.Doctor => await _unitOfWork.DoctorAppointments.AnyAsync(a => a.Doctor.UserId == userId && a.PatientId == patientId),
            DefaultRoles.Nurse => await _unitOfWork.NurseAppointments.AnyAsync(a => a.Nurse.UserId == userId && a.PatientId == patientId),
            DefaultRoles.Lab => await _unitOfWork.LabAppointments.AnyAsync(a => a.Lab.UserId == userId && a.PatientId == patientId),
            _ => false
        };
    }
}
