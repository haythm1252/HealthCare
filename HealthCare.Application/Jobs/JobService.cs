using HealthCare.Application.Interfaces.Repositories.UnitOfWork;
using HealthCare.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Jobs;

public class JobsService(IUnitOfWork unitOfWork, ILogger<JobsService> logger) : IJobsService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ILogger<JobsService> _logger = logger;

    private static readonly DateTime egyptNow = DateTime.UtcNow;
    private readonly DateOnly nowDate = DateOnly.FromDateTime(egyptNow);
    private readonly TimeOnly nowTime = TimeOnly.FromDateTime(egyptNow);
    public async Task MarkPastAppointmentsAsCompleted()
    {
        var doctorApp = await _unitOfWork.DoctorAppointments.AsQueryable()
            .Where(a => a.Status == AppointmentStatus.Confirmed &&
                       (a.DoctorSlot.Date < nowDate || (a.DoctorSlot.Date == nowDate && a.DoctorSlot.EndTime < nowTime)))
            .ExecuteUpdateAsync(s => s.SetProperty(a => a.Status, AppointmentStatus.Completed));

        var nurseApp = await _unitOfWork.NurseAppointments.AsQueryable()
            .Where(a => a.Status == AppointmentStatus.Confirmed &&
                       (a.NurseShift.Date < nowDate ||
                       (a.NurseShift.Date == nowDate &&
                        // to handle both types of appointmetn if its quick visit or hourly stay
                        a.StartTime.AddMinutes(a.Hours.HasValue ? a.Hours.Value * 60 : 30) < nowTime)))
            .ExecuteUpdateAsync(s => s.SetProperty(a => a.Status, AppointmentStatus.Completed));

        var labApp = await _unitOfWork.LabAppointments.AsQueryable()
            .Where(a => a.Status == AppointmentStatus.Confirmed &&
                       (a.Date < nowDate || (a.Date == nowDate && a.StartTime.AddMinutes(30) < nowTime)))
            .ExecuteUpdateAsync(s => s.SetProperty(a => a.Status, AppointmentStatus.Completed));

        _logger.LogWarning("Marked {DoctorCount} doctor appointments, {NurseCount} nurse appointments, {LabCount} lab appointments as completed.", doctorApp, nurseApp, labApp);
    }


    // this method for marking the pending appointments in home visits  that the provider(doctor,nurse,lab)
    // dose not confirm it or decliend it  in home visits as DECLINED 
    // Also handle the pending online requests that never paid
    public async Task MarkPastPendingAppointmentsAsDeclined()
    {
        // this for home visits 
        var homeVisitDoctorApp = await _unitOfWork.DoctorAppointments.AsQueryable()
            .Where(a => a.Status == AppointmentStatus.Pending && a.AppointmentType == AppointmentType.HomeVisit &&
                       (a.DoctorSlot.Date < nowDate || (a.DoctorSlot.Date == nowDate && a.DoctorSlot.EndTime < nowTime)))
            .ExecuteUpdateAsync(s => s.SetProperty(a => a.Status, AppointmentStatus.Declined));

        // this for online appointments
        var onlineDoctorApp = await _unitOfWork.DoctorAppointments.AsQueryable()
            .Where(a => a.Status == AppointmentStatus.Pending && a.AppointmentType == AppointmentType.Online &&
                       (a.DoctorSlot.Date < nowDate || (a.DoctorSlot.Date == nowDate && a.DoctorSlot.EndTime < nowTime)))
            .ExecuteUpdateAsync(s => s.SetProperty(a => a.Status, AppointmentStatus.Cancelled));

        _logger.LogWarning("Marked {Count} online doctor appointments as cancelled.", onlineDoctorApp);


        var nurseApp = await _unitOfWork.NurseAppointments.AsQueryable()
            .Where(a => a.Status == AppointmentStatus.Pending &&
                       (a.NurseShift.Date < nowDate ||
                       (a.NurseShift.Date == nowDate &&
                        // to handle both types of appointmetn if its quick visit or hourly stay
                        a.StartTime.AddMinutes(a.Hours.HasValue ? a.Hours.Value * 60 : 30) < nowTime)))
            .ExecuteUpdateAsync(s => s.SetProperty(a => a.Status, AppointmentStatus.Declined));

        var labApp = await _unitOfWork.LabAppointments.AsQueryable()
            .Where(a => a.Status == AppointmentStatus.Pending &&
                       (a.Date < nowDate || (a.Date == nowDate && a.StartTime.AddMinutes(30) < nowTime)))
            .ExecuteUpdateAsync(s => s.SetProperty(a => a.Status, AppointmentStatus.Declined));

        _logger.LogWarning("Marked {LabCount} lab appointments, {HomeVisitDoctorCount} home visit doctor appointments, {NurseCount} nurse appointments as declined.", labApp, homeVisitDoctorApp, nurseApp);
    }

    public async Task CleanupUnbookedSlotsAndShifts()
    {
        var slotRes = await _unitOfWork.DoctorSlots.DeleteSlots();
        var shiftRes = await _unitOfWork.NurseShifts.DeleteShifts();

        _logger.LogWarning("Slots to delete: {DoctorSlotsToDelete} doctor slots, {NurseShiftsToDelete} nurse shifts.", slotRes.slotsToDel, shiftRes.shiftsToDel);
        _logger.LogWarning("Deleted {DoctorSlotsCount} doctor slots and {NurseShiftsCount} nurse shifts.", slotRes.slotsDeleted, shiftRes.shiftsDeleted);
    }
}
