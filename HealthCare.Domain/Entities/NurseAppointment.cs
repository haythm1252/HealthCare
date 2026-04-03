using HealthCare.Domain.Enums;
using HealthCare.Domain.Users;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Domain.Entities;

public sealed class NurseAppointment : BaseEntity
{
    public string? Notes { get; set; }
    public string Address { get; set; } = string.Empty;
    public TimeOnly StartTime { get; set; } = default;
    public AppointmentStatus Status { get; set; }
    public NurseServiceType ServiceType { get; set; }
    public int? Hours { get; set; }
    public decimal TotalFee { get; set; }

    public Guid NurseId { get; set; }
    public Nurse Nurse { get; set; } = default!;
    public Guid PatientId { get; set; }
    public Patient Patient { get; set; } = default!;
    public Guid NurseShiftId { get; set; }
    public NurseShift NurseShift { get; set; } = default!;
}
