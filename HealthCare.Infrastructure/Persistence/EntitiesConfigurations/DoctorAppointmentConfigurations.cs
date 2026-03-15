using HealthCare.Domain.Entities;
using HealthCare.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HealthCare.Infrastructure.Persistence.EntitiesConfigurations;

public class DoctorAppointmentConfigurations : IEntityTypeConfiguration<DoctorAppointment>
{
    public void Configure(EntityTypeBuilder<DoctorAppointment> builder)
    {

        builder.HasOne(x => x.DoctorSlot)
            .WithOne(x => x.DoctorAppointment)
            .HasForeignKey<DoctorAppointment>(x => x.DoctorSlotId);

        builder.Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(x => x.AppointmentType)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(x => x.PaymentStatus)
            .HasConversion<string>()
            .HasMaxLength(40);

        builder.Property(x => x.Fee).HasPrecision(18, 2);

        
    }
}
