using HealthCare.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Infrastructure.Persistence.EntitiesConfigurations;

public class DoctorAppointmentTestConfigurations : IEntityTypeConfiguration<DoctorAppointmentTest>
{
    public void Configure(EntityTypeBuilder<DoctorAppointmentTest> builder)
    {
        builder.Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(20);
    }
}
