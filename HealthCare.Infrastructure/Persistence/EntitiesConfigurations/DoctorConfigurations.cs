using HealthCare.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HealthCare.Infrastructure.Persistence.EntitiesConfigurations;

public class DoctorConfigurations : IEntityTypeConfiguration<Doctor>
{
    public void Configure(EntityTypeBuilder<Doctor> builder)
    {
        builder.HasOne(d => d.User)
            .WithOne()
            .HasForeignKey<Doctor>(d => d.UserId);

        builder.Property(x => x.Bio).HasMaxLength(1000);
        builder.Property(x => x.Title).HasMaxLength(200);
        builder.Property(x => x.ProfilePictureUrl).HasMaxLength(500);
        builder.Property(x => x.ProfilePicturePublicId).HasMaxLength(200);
        builder.Property(x => x.ClinicFee).HasPrecision(18, 2);
        builder.Property(x => x.OnlineFee).HasPrecision(18, 2);
        builder.Property(x => x.HomeFee).HasPrecision(18, 2);
        builder.Property(x => x.Rating).HasPrecision(3, 2);
        builder.Property(x => x.Balance).HasPrecision(18, 2);

        
    }
}
