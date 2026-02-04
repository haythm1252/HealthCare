using HealthCare.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Domain.Users;

public sealed class Lab : BaseEntity
{
    public string Bio { get; set; } = string.Empty;
    public string ProfilePictureUrl { get; set; } = string.Empty;
    public string ProfilePicturePublicId { get; set; } = string.Empty;
    public decimal Rating { get; set; }
    public decimal HomeVisitFee { get; set; }

    public string UserId { get; set; } = string.Empty;
    public ApplicationUser User { get; set; } = default!;

    public ICollection<LabAppointment> LabAppointments { get; set; } = [];
    public ICollection<LabShift> LabShifts { get; set; } = [];
    public ICollection<Review> Reviews { get; set; } = [];
    public ICollection<LabTest> LabTests { get; set; } = [];
}
