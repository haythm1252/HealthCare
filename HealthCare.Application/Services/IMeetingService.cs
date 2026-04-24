using HealthCare.Application.Common.Result;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Services;

public interface IMeetingService
{
    Task<Result<string>> CreateMeetingAsync(Guid appointmentId, DateTime endDateTime);
}
