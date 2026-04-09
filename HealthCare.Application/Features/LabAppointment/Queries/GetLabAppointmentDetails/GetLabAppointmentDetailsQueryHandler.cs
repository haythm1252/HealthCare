using HealthCare.Application.Common.Consts;
using HealthCare.Application.Common.Result;
using HealthCare.Application.Errors;
using HealthCare.Application.Features.DoctorAppointments.Contracts;
using HealthCare.Application.Features.LabAppointment.Contracts;
using HealthCare.Application.Interfaces.Repositories.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.LabAppointment.Queries.GetLabAppointmentDetails;

public class GetLabAppointmentDetailsQueryHandler(IUnitOfWork unitOfWork) 
    : IRequestHandler<GetLabAppointmentDetailsQuery, Result<LabAppointmentDetailsResponse>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result<LabAppointmentDetailsResponse>> Handle(GetLabAppointmentDetailsQuery request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.LabAppointments.AsQueryable()
                    .AsNoTracking()
                    .AsSplitQuery()
                    .Where(a => a.Id == request.AppointmentId);

        if (request.UserRole == DefaultRoles.Patient)
            query = query.Where(a => a.Patient.UserId == request.UserId);
        else if (request.UserRole == DefaultRoles.Lab)
            query = query.Where(a => a.Lab.UserId == request.UserId);
        else
            return Result.Failure<LabAppointmentDetailsResponse>(AppointmentErrors.NotFound);


        var appointment = await query
            .Select(a => new LabAppointmentDetailsResponse(
                a.Id,
                a.LabId,
                a.PatientId,
                a.Lab.User.Name,
                a.AppointmentType,
                a.Status,
                a.Date,
                a.StartTime,
                a.TotalFee,
                a.Notes,
                a.Address,

                a.TestResults.Select(tr => new TestResultDto(
                    tr.TestId,
                    tr.Test.Name,
                    tr.Value,
                    tr.Summary,
                    tr.ResultFileUrl,
                    tr.Status,
                    tr.SubmittedAt
                ))
            )).SingleOrDefaultAsync(cancellationToken);

        if (appointment is null)
            return Result.Failure<LabAppointmentDetailsResponse>(AppointmentErrors.NotFound);

        return Result.Success(appointment);
    }
}

