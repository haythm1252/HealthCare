using HealthCare.Application.Common.Result;
using HealthCare.Application.Errors;
using HealthCare.Application.Features.Appointments.Contracts;
using HealthCare.Application.Interfaces.Repositories.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.Appointments.Queries.PatientAppointmentHistory;

public class PatientAppointmentHistoryQueryHandler(IUnitOfWork unitOfWork) 
    : IRequestHandler<PatientAppointmentHistoryQuery, Result<PatientAppointmentHistoryResponse>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result<PatientAppointmentHistoryResponse>> Handle(PatientAppointmentHistoryQuery request, CancellationToken cancellationToken)
    {
        var patientId = await _unitOfWork.Patients.AsQueryable()
            .Where(p => p.UserId == request.UserId)
            .Select(p => p.Id)
            .SingleOrDefaultAsync(cancellationToken);

        if(patientId == Guid.Empty)
            return Result.Failure<PatientAppointmentHistoryResponse>(UserErrors.NotFound);

        var doctorAppointments = await _unitOfWork.DoctorAppointments.GetPatientAppointmentsAsync(patientId, cancellationToken);
        var nurseAppointments = await _unitOfWork.NurseAppointments.GetPatientAppointmentsAsync(patientId, cancellationToken);
        var labAppointments = await _unitOfWork.LabAppointments.GetPatientAppointmentsAsync(patientId, cancellationToken);

        var historyResponse = new PatientAppointmentHistoryResponse(
                doctorAppointments,
                nurseAppointments,
                labAppointments
            );

        return Result.Success(historyResponse);
    }
}
