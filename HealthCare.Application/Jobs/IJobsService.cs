using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Jobs;

public interface IJobsService
{
    Task MarkPastAppointmentsAsCompleted();
    Task CleanupUnbookedSlotsAndShifts();
    Task MarkPastPendingAppointmentsAsDeclined();
}
