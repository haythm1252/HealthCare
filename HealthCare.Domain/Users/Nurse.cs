using HealthCare.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Domain.Users;

public sealed class Nurse : BaseEntity
{
    public string Bio { get; set; } = string.Empty;
    public string ProfilePictureUrl { get; set; } = string.Empty;
    public string ProfilePicturePublicId { get; set; } = string.Empty;
    public decimal Rating { get; set; }
    public decimal HomeVisitFee { get; set; }

    public string UserId { get; set; } = string.Empty;
    public ApplicationUser User { get; set; } = default!;

    public ICollection<NurseAppointment> NurseAppointments { get; set; } = [];
    public ICollection<NurseShift> NurseShifts { get; set; } = [];
    public ICollection<Review> Reviews { get; set; } = [];
}
