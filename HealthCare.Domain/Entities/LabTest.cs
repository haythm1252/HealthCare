using HealthCare.Domain.Users;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Domain.Entities;

public sealed class LabTest : BaseEntity
{
    public decimal Price { get; set; }
    public bool IsAvailableAtHome { get; set; }

    public Guid LabId { get; set; }
    public Lab Lab { get; set; } = default!;

    public Guid TestId { get; set; }
    public Test Test { get; set; } = default!;
}
