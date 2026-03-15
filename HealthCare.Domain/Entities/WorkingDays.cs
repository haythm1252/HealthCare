using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Domain.Entities;

public sealed class WorkingDays
{   
    public bool IsSaturdayOpen { get; set; } = true;
    public bool IsSundayOpen { get; set; } = true;
    public bool IsMondayOpen { get; set; } = true;
    public bool IsTuesdayOpen { get; set; } = true;
    public bool IsWednesdayOpen { get; set; } = true;
    public bool IsThursdayOpen { get; set; } = true;
    public bool IsFridayOpen { get; set; }

}
