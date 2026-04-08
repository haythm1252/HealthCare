using HealthCare.Application.Features.Auth.Contracts;
using HealthCare.Application.Features.Patients.Contracts;
using HealthCare.Application.Features.Patients.MedicalRecordContracts;
using HealthCare.Application.Features.Patients.Queries.PatientProfile;
using HealthCare.Application.Interfaces.Repositories;
using HealthCare.Domain.Enums;
using HealthCare.Domain.Users;
using HealthCare.Infrastructure.Persistence;
using HealthCare.Infrastructure.Repositories.Base;
using Mapster;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Infrastructure.Repositories;

public class PatientRepository(ApplicationDbContext context) : BaseRepository<Patient>(context), IPatientRepository
{
    private readonly ApplicationDbContext _context = context;

    public async Task<MedicalRecordResponse?> GetPatientMedicalRecordAsync(Guid patientId, CancellationToken cancellationToken)
    {
        var medicalRecord = await _context.Patients
            .AsNoTracking()
            .AsSplitQuery()
            .Where(p => p.Id == patientId)
            .Select(p => new
            {
                p.User.Name,
                p.DateOfBirth,
                p.User.Gender,
                p.Weight,

                Diseases = new DiseasesDto(
                    p.HasDiabetes,
                    p.HasBloodPressure,
                    p.HasAsthma,
                    p.HasHeartDisease,
                    p.HasKidneyDisease,
                    p.HasArthritis,
                    p.HasCancer,
                    p.HasHighCholesterol,
                    p.OtherMedicalConditions
                ),

                Diagnoses = p.DoctorAppointments
                    .Where(a => a.Status == AppointmentStatus.Completed)
                    .Select(a => new DiagnosisDto(
                        a.Id,
                        a.Doctor.User.Name,
                        a.DoctorSlot.Date,
                        a.AppointmentType,
                        a.Diagnosis,
                        a.Prescriptions,
                        a.DoctorAppointmentTests
                            .Select(rt => new RequiredTestDto(
                                rt.TestId,
                                rt.Test.Name,
                                rt.Status
                            ))
                    )),

                LabResults = p.LabAppointments
                    .Where(a => a.Status == AppointmentStatus.ResultsDone)
                    .Select(a => new LabAppointmentTestResultsDto(
                        a.Id,
                        a.Lab.User.Name,
                        a.Date,
                        a.AppointmentType,
                        a.TestResults
                            .Select(tr => new TestResultDto(
                                tr.Id,
                                tr.Test.Name,
                                tr.Value,
                                tr.Summary,
                                tr.ResultFileUrl!,
                                tr.Status,
                                tr.SubmittedAt
                            ))
                    ))
            }).SingleOrDefaultAsync(cancellationToken);

        if (medicalRecord is null)
            return null;

        var pendingTests = await _context.DoctorAppointmentTests
        .AsNoTracking()
        .Where(dat => dat.DoctorAppointment.PatientId == patientId && dat.DoctorAppointment.Status == AppointmentStatus.Completed 
            && dat.Status == TestResultStatus.Pending)
        .Select(dat => new RequiredTestDto(
            dat.TestId,
            dat.Test.Name,
            dat.Status
        ))
        .Distinct()
        .ToListAsync(cancellationToken);

        return new MedicalRecordResponse(
            medicalRecord!.Name,
            medicalRecord.DateOfBirth,
            medicalRecord.Gender,
            medicalRecord.Weight,
            medicalRecord.Diseases,
            medicalRecord.Diagnoses,
            medicalRecord.LabResults,
            pendingTests
        );

    }
}
