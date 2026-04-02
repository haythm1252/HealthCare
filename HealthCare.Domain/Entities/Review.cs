using HealthCare.Domain.Enums;
using HealthCare.Domain.Users;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Domain.Entities;

public sealed class Review : BaseEntity
{
    public int Rating { get; set; }
    public string? Comment { get; set; } 
    public Guid PatientId { get; set; }
    public Patient Patient { get; set; } = default!;
    public Guid TargetId { get; set; }
    public TargetType TargetType { get; set; }
}
