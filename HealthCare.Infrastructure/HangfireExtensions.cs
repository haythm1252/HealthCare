using Hangfire;
using HealthCare.Application.Jobs;
using Microsoft.Extensions.DependencyInjection;


namespace HealthCare.Infrastructure;

public static class HangfireExtensions
{
    public static void AddHangfireRecurringJobs(this IServiceProvider serviceProvider)
    {
        var recurringJobManager = serviceProvider.GetRequiredService<IRecurringJobManager>();

        recurringJobManager.AddOrUpdate<IJobsService>(
            "Mark-Past-Appointments-As-Completed",
            job => job.MarkPastAppointmentsAsCompleted(),
            Cron.Hourly()
        );

        recurringJobManager.AddOrUpdate<IJobsService>(
            "Mark-Past-Pending-Appointments-As-Declined",
            job => job.MarkPastPendingAppointmentsAsDeclined(),
            Cron.Hourly()
        );

        recurringJobManager.AddOrUpdate<IJobsService>(
            "Cleanup-Unbooked-Slots-And-Shifts",
            job => job.CleanupUnbookedSlotsAndShifts(),
            Cron.Daily(2)
        );
    }
}
