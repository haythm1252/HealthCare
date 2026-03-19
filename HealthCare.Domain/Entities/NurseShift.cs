using HealthCare.Domain.Users;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Domain.Entities;

public sealed class NurseShift : BaseEntity
{
    public DateOnly Date { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public bool IsBooked { get; set; }
    public Guid NurseId { get; set; }
    public Nurse Nurse { get; set; } = default!;

    public ICollection<NurseAppointment> NurseAppointments { get; set; } = [];
}
