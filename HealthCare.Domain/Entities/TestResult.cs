using HealthCare.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Domain.Entities;

public sealed class TestResult : BaseEntity
{
    public decimal? Value { get; set; }
    public string? Summary { get; set; }
    public string? AttachmentUrl { get; set; }
    public string? AttachmentPublicId { get; set; }
    public TestResultStatus Status { get; set; }
    public DateTime? SubmittedAt { get; set; }
    public Guid TestId { get; set; }
    public Test Test { get; set; } = default!;

    public Guid LabAppointmentId { get; set; }
    public LabAppointment LabAppointment { get; set; } = default!;
}
