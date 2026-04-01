using HealthCare.Application.Common.Consts;
using HealthCare.Application.Common.Result;
using HealthCare.Application.Errors;
using HealthCare.Application.Features.LabAppointment.Contracts;
using HealthCare.Application.Features.LabTest.Contracts;
using HealthCare.Application.Interfaces.Repositories.UnitOfWork;
using HealthCare.Application.Services;
using HealthCare.Domain.Entities;
using HealthCare.Domain.Enums;
using HealthCare.Domain.Users;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.LabAppointment.Commands.BookLabAppointment;

public class BookLabAppointmentCommandHandler(IUnitOfWork unitOfWork, INotificationService notificationService)
    : IRequestHandler<BookLabAppointmentCommand, Result<BookLabAppointmentResponse>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly INotificationService _notificationService = notificationService;

    public async Task<Result<BookLabAppointmentResponse>> Handle(BookLabAppointmentCommand request, CancellationToken cancellationToken)
    {
        var patient = await _unitOfWork.Patients.AsQueryable()
            .Where(p => p.UserId == request.UserId)
            .Select(p => new { p.Id, p.User.Name })
            .SingleOrDefaultAsync(cancellationToken);

        if (patient is null)
            return Result.Failure<BookLabAppointmentResponse>(UserErrors.NotFound);

        //dose the lab exist 
        var lab = await _unitOfWork.Labs.AsQueryable()
            .Where(l => l.Id == request.LabId)
            .Select(l => new { l.Id, l.User.Email, l.User.Name, l.User.PhoneNumber, l.User.Address, l.HomeVisitFee })
            .SingleOrDefaultAsync(cancellationToken);

        if (lab is null)
            return Result.Failure<BookLabAppointmentResponse>(LabErrors.NotFound);

        // dose the lab contain those tests 
        var requestedTests = await _unitOfWork.LabTests.AsQueryable()
            .Where(lt => lt.LabId == request.LabId && request.LabTestsIds.Contains(lt.Id))
            .ProjectToType<LabTestResponse>()
            .ToListAsync(cancellationToken);

        if (requestedTests.Count != request.LabTestsIds.Count())
            return Result.Failure<BookLabAppointmentResponse>(LabAppointmentErrors.TestsNotExisit);

        // if the appointment type is home visit check if there is test dose not support homve visit
        var isHomeVisit = request.AppointmentType.Equals(nameof(AppointmentType.HomeVisit), StringComparison.OrdinalIgnoreCase);
        if (isHomeVisit && requestedTests.Any(lt => !lt.IsAvailableAtHome))
            return Result.Failure<BookLabAppointmentResponse>(LabAppointmentErrors.NotSupportHome);

        //dose the patient have already an appointment in this day and not canceled
        var alreadyBooked = await _unitOfWork.LabAppointments.AsQueryable()
            .AnyAsync(a => a.PatientId == patient.Id && a.LabId == lab.Id 
                && a.Date == request.Date && a.Status != AppointmentStatus.Cancelled, cancellationToken);

        if (alreadyBooked)
            return Result.Failure<BookLabAppointmentResponse>(LabAppointmentErrors.DuplicateBooking);


        // adding the appointment in the database
        var totalFee = isHomeVisit 
            ? requestedTests.Sum(t => t.Price) + lab.HomeVisitFee
            : requestedTests.Sum(t => t.Price);

        var appointment = new Domain.Entities.LabAppointment
        {
            PatientId = patient.Id,
            LabId = lab.Id,
            
            Date = request.Date,
            Notes = request.Notes,
            Address = request.Address,
            TotalFee = totalFee,
            AppointmentType = Enum.Parse<AppointmentType>(request.AppointmentType,true),
            Status = isHomeVisit ? AppointmentStatus.Pending : AppointmentStatus.Confirmed,

            TestResults = requestedTests.Select(t => new TestResult
            {
                TestId = t.TestId,
                Status = TestResultStatus.Pending
            }).ToList()
        };

        await _unitOfWork.LabAppointments.AddAsync(appointment, cancellationToken);
        var res = await _unitOfWork.SaveChangesAsync(cancellationToken);

        if(res <= 0)
            return Result.Failure<BookLabAppointmentResponse>(LabAppointmentErrors.SaveFailed);

        //sending email notificaton and the response
        await _notificationService.SendNewAppointmentNotificationAsync(
            lab.Email!,
            lab.Name,
            DefaultRoles.Lab,
            patient.Name,
            appointment.Date.ToString(),
            appointment.AppointmentType.ToString()
        );

        var response = new BookLabAppointmentResponse(
            appointment.Id,
            lab.Name,
            lab.PhoneNumber!,
            appointment.Date,
            isHomeVisit ? request.Address! : lab.Address,
            appointment.AppointmentType,
            appointment.TotalFee,
            appointment.Status,
            requestedTests
        );

        return Result.Success(response);
    }
}
