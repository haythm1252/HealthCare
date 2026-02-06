using HealthCare.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HealthCare.Infrastructure.Persistence.EntitiesConfigurations;

public class NurseConfigurations : IEntityTypeConfiguration<Nurse>
{
    public void Configure(EntityTypeBuilder<Nurse> builder)
    {
        builder.HasOne(n => n.User)
            .WithOne()
            .HasForeignKey<Nurse>(n => n.UserId);

        builder.Property(x => x.Bio).HasMaxLength(1000);
        builder.Property(x => x.ProfilePictureUrl).HasMaxLength(500);
        builder.Property(x => x.ProfilePicturePublicId).HasMaxLength(200);
        builder.Property(x => x.Rating).HasPrecision(3, 2);
        builder.Property(x => x.HomeVisitFee).HasPrecision(18, 2);

        
    }
}
