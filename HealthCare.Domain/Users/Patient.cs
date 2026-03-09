using HealthCare.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Domain.Users;

public sealed class Patient : BaseUser
{
    public DateOnly DateOfBirth { get; set; }
    public bool HasDiabetes { get; set; }
    public bool HasBloodPressure { get; set; }
    public bool HasAsthma { get; set; }
    public bool HasHeartDisease { get; set; }
    public bool HasKidneyDisease { get; set; }
    public bool HasArthritis { get; set; }
    public bool HasCancer { get; set; }
    public bool HasHighCholesterol { get; set; }
    public string? OtherMedicalConditions { get; set; }
    public decimal? Weight { get; set; }
    public string UserId { get; set; } = string.Empty;
    public ApplicationUser User { get; set; } = default!;

    public ICollection<LabAppointment> LabAppointments { get; set; } = [];
    public ICollection<DoctorAppointment> DoctorAppointments { get; set; } = [];
    public ICollection<NurseAppointment> NurseAppointments { get; set; } = [];
    public ICollection<Chat> Chats { get; set; } = [];
    public ICollection<Review> Reviews { get; set; } = [];
}
