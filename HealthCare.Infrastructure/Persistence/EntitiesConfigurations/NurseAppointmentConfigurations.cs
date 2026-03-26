using HealthCare.Domain.Entities;
using HealthCare.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HealthCare.Infrastructure.Persistence.EntitiesConfigurations;

public class NurseAppointmentConfigurations : IEntityTypeConfiguration<NurseAppointment>
{
    public void Configure(EntityTypeBuilder<NurseAppointment> builder)
    {
        builder.Property(x => x.Address).IsRequired().HasMaxLength(250);

        builder.Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(20);


        builder.Property(x => x.ServiceType)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(x => x.TotalFee).HasPrecision(18, 2);

        
    }
}
