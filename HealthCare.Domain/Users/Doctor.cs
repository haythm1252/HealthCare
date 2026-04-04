using HealthCare.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Domain.Users;

public sealed class Doctor : BaseUser
{
    public string Bio { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string ProfilePictureUrl { get; set; } = string.Empty;
    public string ProfilePicturePublicId { get; set; } = string.Empty;
    public decimal ClinicFee { get; set; }
    public decimal OnlineFee { get; set; }
    public decimal HomeFee { get; set; }
    public decimal Rating { get; set; }
    public int RatingsCount { get; set; }

    public bool AllowHomeVisit { get; set; } = false;
    public bool AllowOnlineConsultation { get; set; } = false;
    
    public string UserId { get; set; } = string.Empty;
    public ApplicationUser User { get; set; } = default!;

    public Guid SpecialtyId { get; set; }
    public Specialty Specialty { get; set; } = default!;

    public ICollection<DoctorAppointment> DoctorAppointments { get; set; } = [];
    public ICollection<DoctorSlot> DoctorSlots { get; set; } = [];
    public ICollection<Chat> Chats { get; set; } = [];
    public ICollection<Post> Posts { get; set; } = [];
}
