using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Domain.Enums;

public enum AppointmentStatus
{
    Pending,
    Confirmed,
    Declined,
    Cancelled,
    Completed,
    NoShow,
    ResultsDone //for lab
}
