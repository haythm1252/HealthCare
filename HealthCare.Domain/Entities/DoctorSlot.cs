using HealthCare.Domain.Users;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Domain.Entities;

public sealed class DoctorSlot : BaseEntity
{
    public DateTime StartDate { get; set; }
    public int DurationInMinutes { get; set; }
    public bool IsBooked { get; set; } = false;

    public Guid DoctorId { get; set; }
    public Doctor Doctor { get; set; } = default!;
    public DoctorAppointment? DoctorAppointment { get; set; }

}
