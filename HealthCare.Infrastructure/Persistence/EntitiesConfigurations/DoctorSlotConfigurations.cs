using HealthCare.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HealthCare.Infrastructure.Persistence.EntitiesConfigurations;

public class DoctorSlotConfigurations : IEntityTypeConfiguration<DoctorSlot>
{
    public void Configure(EntityTypeBuilder<DoctorSlot> builder)
    {
        builder.HasIndex(x => new { x.DoctorId, x.Date, x.StartTime }).IsUnique();
    }
}
