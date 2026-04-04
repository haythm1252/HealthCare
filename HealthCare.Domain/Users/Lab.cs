using HealthCare.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Domain.Users;

public sealed class Lab : BaseUser
{
    public string Bio { get; set; } = string.Empty;
    public string ProfilePictureUrl { get; set; } = string.Empty;
    public string ProfilePicturePublicId { get; set; } = string.Empty;
    public decimal Rating { get; set; }
    public int RatingsCount { get; set; }
    public decimal HomeVisitFee { get; set; }
    public TimeOnly OpeningTime { get; set; }
    public TimeOnly ClosingTime { get; set; }
    public WorkingDays WorkingDays { get; set; } = new();

    public string UserId { get; set; } = string.Empty;
    public ApplicationUser User { get; set; } = default!;

    public ICollection<LabAppointment> LabAppointments { get; set; } = [];
    public ICollection<LabTest> LabTests { get; set; } = [];
}
