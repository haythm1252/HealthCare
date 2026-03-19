using HealthCare.Domain.Users;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Domain.Entities;

public sealed class DoctorSlot : BaseEntity
{

    public DateOnly Date { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }   
    public bool IsBooked { get; set; } = false;

    public Guid DoctorId { get; set; }
    public Doctor Doctor { get; set; } = default!;
    public DoctorAppointment? DoctorAppointment { get; set; }

}
