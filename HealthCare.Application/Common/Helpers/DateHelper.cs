using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace HealthCare.Application.Common.Helpers;

public static class DateHelper
{
    public static DateTime GetEgyptTime(this DateTime utcDateTime)
    {
        var tzId = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                   ? "Egypt Standard Time"
                   : "Africa/Cairo";

        var egyptTimeZone = TimeZoneInfo.FindSystemTimeZoneById(tzId);
        return TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, egyptTimeZone);
    }
}
