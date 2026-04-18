using HealthCare.Application.Common.Result;
using HealthCare.Application.Errors;
using HealthCare.Application.Interfaces.Repositories.UnitOfWork;
using HealthCare.Application.Services;
using HealthCare.Domain.Enums;
using HealthCare.Domain.Users;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace HealthCare.Application.Features.LabAppointment.Commands.AddTestResult;

public class AddTestResultCommandHandler(IUnitOfWork unitOfWork, ICloudinaryService cloudinaryService) : IRequestHandler<AddTestResultCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ICloudinaryService _cloudinaryService = cloudinaryService;


    //get the lab and check
    // get the pateint in this appointment and get his requirement tests
    public async Task<Result> Handle(AddTestResultCommand request, CancellationToken cancellationToken)
    {
        var isLabExist = await _unitOfWork.Labs.AnyAsync(l => l.UserId == request.UserId, cancellationToken);
        if (!isLabExist)
            return Result.Failure(LabErrors.NotFound);

        var testResult = await _unitOfWork.TestResults.AsQueryable()
            .Include(tr => tr.LabAppointment)
            .Where(tr => tr.Id == request.TestResultId && tr.LabAppointmentId == request.AppointmentId)
            .SingleOrDefaultAsync(cancellationToken);

        if (testResult is null)
            return Result.Failure(new Error("TestResult.NotFound", "The test result or the lab appointment does not exist.", 404));

        // added resultsDone so the lab can update even if the appointmetn already have the results
        if(testResult.LabAppointment.Status is not (AppointmentStatus.Completed or AppointmentStatus.ResultsDone))
            return Result.Failure(new Error("LabAppointment.InvalidState", 
                "Cannot upload results until the patient visit is marked as Completed (Samples Collected).", 400));

        // i added this to make the pdf file required in the first time and after this its not required so if just update the summary or smth
        if (string.IsNullOrWhiteSpace(testResult.ResultFileUrl) && request.ResultFile is null)
            return Result.Failure(new Error("TestResult.FileRequired", "A PDF file is required for the initial result upload.", 400));

        if(request.ResultFile is not null)
        {
            using var stream = request.ResultFile!.OpenReadStream();

            var result = await _cloudinaryService.UploadPdfAsync(stream, request.ResultFile.FileName);
            if (result.IsFailure)
                return Result.Failure(result.Error);

            if (!string.IsNullOrWhiteSpace(testResult.ResultFileUrl))
            {
                var deleteingResult = await _cloudinaryService.DeletePdfAsync(testResult.ResultFilePublicId!);
                if (deleteingResult.IsFailure)
                    return Result.Failure(deleteingResult.Error);
            }

            testResult.ResultFileUrl = result.Value.Url;
            testResult.ResultFilePublicId = result.Value.PublicId;
            //i put it here cuz the submittedat is for the file
            testResult.SubmittedAt = DateTime.UtcNow;
        }

        testResult.Value = request.Value;
        testResult.Summary = request.Summary;
        testResult.Status = TestResultStatus.Completed;
        testResult.LastModified = DateTime.UtcNow;


        // If there is No tests are in progress make the appointment statuse resultsdone 
        var hasUnFinishedTests = await _unitOfWork.TestResults.AsQueryable().AnyAsync(tr => tr.LabAppointmentId == testResult.LabAppointmentId
            && tr.Id != testResult.Id && tr.Status != TestResultStatus.Completed, cancellationToken);

        if (!hasUnFinishedTests)
            testResult.LabAppointment.Status = AppointmentStatus.ResultsDone;

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // update all required tests that the doctor ask for it 
        var requiredTest = await _unitOfWork.DoctorAppointmentTests.AsQueryable()
            .Where(rt => rt.DoctorAppointment.PatientId == testResult.LabAppointment.PatientId
                && rt.TestId == testResult.TestId
                && rt.Status != TestResultStatus.Completed)
            .ExecuteUpdateAsync(setters => setters.SetProperty(a => a.Status, TestResultStatus.Completed), cancellationToken);

        return Result.Success();
    }
}
