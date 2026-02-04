using HealthCare.Domain.Users;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Domain.Entities;

public sealed class Chat : BaseEntity
{

    public Guid DoctorId { get; set; }
    public Doctor Doctor { get; set; } = default!;
    public Guid PatientId { get; set; }
    public Patient Patient { get; set; } = default!;

    public ICollection<Message> Messages { get; set; } = [];
}
