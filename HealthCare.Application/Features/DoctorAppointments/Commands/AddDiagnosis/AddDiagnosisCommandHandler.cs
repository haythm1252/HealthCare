using HealthCare.Application.Common.Result;
using HealthCare.Application.Errors;
using HealthCare.Application.Interfaces.Repositories.UnitOfWork;
using HealthCare.Domain.Entities;
using HealthCare.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.DoctorAppointments.Commands.AddDiagnosis;

public class AddDiagnosisCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<AddDiagnosisCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;


    // this command is for adding and Updateing in the same time
    public async Task<Result> Handle(AddDiagnosisCommand request, CancellationToken cancellationToken)
    {
        var doctorId = await _unitOfWork.Doctors.AsQueryable()
            .Where(d => d.UserId == request.UserId)
            .Select(d => d.Id)
            .SingleOrDefaultAsync(cancellationToken);

        if(doctorId == Guid.Empty)
            return Result.Failure(DoctorErrors.NotFound);

        var appointment = await _unitOfWork.DoctorAppointments.AsQueryable()
            .Include(da => da.DoctorSlot)
            .Include(da => da.DoctorAppointmentTests)
            .Where(da => da.Id == request.AppointmentId && da.DoctorId == doctorId)
            .SingleOrDefaultAsync(cancellationToken);

        if(appointment is null)
            return Result.Failure(AppointmentErrors.NotFound);

        if(appointment.DoctorSlot.Date.ToDateTime(appointment.DoctorSlot.StartTime) > DateTime.UtcNow)
            return Result.Failure(DoctorAppointmentErrors.TooEarlyToAddDiagnosis);

        if (appointment.Status is not (AppointmentStatus.Confirmed or AppointmentStatus.Completed))
            return Result.Failure(new Error("DoctorAppointment.InvalidStatusToAddDiagnosis", 
                $"The Appointment status is {appointment.Status} right now, you can only add diagnosis to Confirmed or Completed appointments", 400));

        if(request.RequiredTests is not null)
        {
            if (request.RequiredTests.Any())
            {
                int existingCount = await _unitOfWork.Tests.AsQueryable()
                    .CountAsync(t => request.RequiredTests.Contains(t.Id), cancellationToken);

                if (existingCount != request.RequiredTests.Count())
                    return Result.Failure(TestErrros.InvalidTest);

                // delete tests not in the request tests
                var testsToRemove = appointment.DoctorAppointmentTests
                .Where(dbTest => !request.RequiredTests.Contains(dbTest.TestId))
                .ToList();

                await _unitOfWork.DoctorAppointmentTests.DeleteRange(testsToRemove);


                // add tests not in the database tests
                var existingIds = appointment.DoctorAppointmentTests.Select(rt => rt.TestId).ToList();

                var newTests = request.RequiredTests
                    .Where(id => !existingIds.Contains(id))
                    .Select(id => new DoctorAppointmentTest
                    {
                        DoctorAppointmentId = appointment.Id,
                        TestId = id,
                        Status = TestResultStatus.Pending                   
                    });

                await _unitOfWork.DoctorAppointmentTests.AddRangeAsync(newTests, cancellationToken);
            }
            else // if the request tests is empty [] delete all tests
            {
                await _unitOfWork.DoctorAppointmentTests.DeleteRange(appointment.DoctorAppointmentTests);
            }
            
        }

        appointment.Diagnosis = request.Diagnosis;
        appointment.Prescriptions = request.Prescriptions;
        appointment.Status = AppointmentStatus.Completed;
        
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
