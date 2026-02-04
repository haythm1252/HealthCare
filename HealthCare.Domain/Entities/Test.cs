using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Domain.Entities;

public sealed class Test : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string PreRequisites { get; set; } = string.Empty;
    public string Unit { get; set; } = string.Empty;
    public ICollection<LabTest> LabTests { get; set; } = [];
    public ICollection<TestResult> TestResults { get; set; } = [];
}
