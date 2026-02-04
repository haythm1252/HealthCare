using HealthCare.Domain.Enums;
using HealthCare.Domain.Users;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Domain.Entities;

public sealed class LabAppointment : BaseEntity
{
    public AppointmentStatus Status { get; set; }
    public AppointmentType AppointmentType { get; set; }
    public decimal TotalFee { get; set; }
    public string? Notes { get; set; }
    public string? Address { get; set; }

    public Guid PatientId { get; set; }
    public Patient Patient { get; set; } = default!;
    public Guid LabId { get; set; }
    public Lab Lab { get; set; } = default!;
    public Guid LabShiftId { get; set; }
    public LabShift Labshift { get; set;} = default!;

    public ICollection<TestResult> TestResults { get; set; } = [];
}
