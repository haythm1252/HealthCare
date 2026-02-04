using HealthCare.Domain.Users;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Domain.Entities;

public sealed class Review : BaseEntity
{
    public decimal Rating { get; set; }
    public string Comment { get; set; } = string.Empty;
    public Guid PatientId { get; set; }
    public Patient Patient { get; set; } = default!;
    public Guid? DoctorId { get; set; }
    public Doctor? Doctor { get; set; }
    public Guid? NurseId { get; set; }
    public Nurse? Nurse { get; set; }
    public Guid? LabId { get; set; }
    public Lab? Lab { get; set; }
}
