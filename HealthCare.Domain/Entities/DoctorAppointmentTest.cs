using HealthCare.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Domain.Entities;

public sealed class DoctorAppointmentTest : BaseEntity
{

    public TestResultStatus Status { get; set; }
    public Guid DoctorAppointmentId { get; set; }
    public DoctorAppointment DoctorAppointment { get; set; } = default!;

    public Guid TestId { get; set; }
    public Test Test { get; set; } = default!;
}
