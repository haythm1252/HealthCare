using HealthCare.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Services;

public interface INotificationService
{
    Task SendNewAppointmentNotificationAsync(string recipientEmail,
        string recipientName,
        string recipientType,
        string patientName,
        string appointmentDate,
        string serviceName);
}
