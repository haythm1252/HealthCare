using HealthCare.Domain.Enums;
using HealthCare.Domain.Users;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Domain.Entities;

public sealed class DoctorAppointment : BaseEntity
{
    public string? Notes { get; set; }
    public AppointmentStatus Status { get; set; }
    public AppointmentType AppointmentType { get; set; }
    public decimal Fee { get; set; }
    public string? Diagnosis { get; set; } 
    public string? Prescriptions { get; set; } 
    public string? RequiredTests { get; set; }
    public string? Address { get; set; }
    public PaymentType PaymentType { get; set; }

    public string? PaymentOrderId { get; set; }
    public string? PaymentTransactionId { get;set;  }
    public PaymentStatus PaymentStatus { get; set; }
    public DateTime? PaymentDate { get; set; }


    public Guid DoctorSlotId { get; set; }
    public DoctorSlot DoctorSlot { get; set; } = default!;
    public Guid PatientId { get; set; }
    public Patient Patient { get; set; } = default!;
    public Guid DoctorId { get; set; }
    public Doctor Doctor { get; set; } = default!;
}
