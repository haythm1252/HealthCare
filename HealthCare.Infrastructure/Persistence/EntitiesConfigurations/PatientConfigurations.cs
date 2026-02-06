using HealthCare.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HealthCare.Infrastructure.Persistence.EntitiesConfigurations;

public class PatientConfigurations : IEntityTypeConfiguration<Patient>
{
    public void Configure(EntityTypeBuilder<Patient> builder)
    {
        builder.HasOne(x => x.User)
            .WithOne()
            .HasForeignKey<Patient>(x => x.UserId);

        builder.Property(x => x.Weight).HasPrecision(5,2);
        builder.Property(x => x.DateOfBirth).IsRequired();
        
    }
}
