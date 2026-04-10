using HealthCare.Application.Common.Consts;
using HealthCare.Application.Common.Result;
using HealthCare.Application.Errors;
using HealthCare.Application.Features.LabAppointment.Contracts;
using HealthCare.Application.Features.NurseAppointments.Contracts;
using HealthCare.Application.Features.Reviews.Contracts;
using HealthCare.Application.Interfaces.Repositories.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.NurseAppointments.Queries.GetNurseAppointmentDetails;

public class GetNurseAppointmentDetailsQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetNurseAppointmentDetailsQuery, Result<NurseAppointmentDetailsResponse>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result<NurseAppointmentDetailsResponse>> Handle(GetNurseAppointmentDetailsQuery request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.NurseAppointments.AsQueryable()
                    .AsNoTracking()
                    .AsSplitQuery()
                    .Where(a => a.Id == request.AppointmentId);

        if (request.UserRole == DefaultRoles.Patient)
            query = query.Where(a => a.Patient.UserId == request.UserId);
        else if (request.UserRole == DefaultRoles.Nurse)
            query = query.Where(a => a.Nurse.UserId == request.UserId);
        else
            return Result.Failure<NurseAppointmentDetailsResponse>(AppointmentErrors.NotFound);


        var appointment = await query
            .Select(a => new NurseAppointmentDetailsResponse(
                a.Id,
                a.NurseId,
                a.PatientId,
                a.Nurse.User.Name,
                a.ServiceType,
                a.Status,
                a.NurseShift.Date,
                a.NurseShift.StartTime,
                a.NurseShift.EndTime,
                a.StartTime,
                a.TotalFee,
                a.Notes,
                a.Address,
                a.Hours,
                a.Patient.Reviews.Where(r => r.TargetId == a.NurseId)
                    .Select(r => new ReviewResponse(
                        r.Id,
                        r.Patient.User.Name,
                        r.Rating,
                        r.Comment,
                        r.LastModified ?? r.CreatedAt,
                        r.LastModified.HasValue && r.LastModified > r.CreatedAt
                    )).SingleOrDefault()
            )).SingleOrDefaultAsync(cancellationToken);

        if (appointment is null)
            return Result.Failure<NurseAppointmentDetailsResponse>(AppointmentErrors.NotFound);

        return Result.Success(appointment);
    }
}
