using HealthCare.Domain.Users;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Domain.Entities;

public sealed class LabShift : BaseEntity
{
    public DateOnly Date { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }

    public Guid LabId { get; set; }
    public Lab Lab { get; set; } = default!;
    public ICollection<LabAppointment> LabAppointments { get; set; } = [];  
}
