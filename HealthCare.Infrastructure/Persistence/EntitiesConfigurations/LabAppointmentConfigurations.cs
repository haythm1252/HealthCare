using HealthCare.Domain.Entities;
using HealthCare.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HealthCare.Infrastructure.Persistence.EntitiesConfigurations;

public class LabAppointmentConfigurations : IEntityTypeConfiguration<LabAppointment>
{
    public void Configure(EntityTypeBuilder<LabAppointment> builder)
    {
        builder.Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(x => x.AppointmentType)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(x => x.TotalFee).HasPrecision(18, 2);

        
    }
}
