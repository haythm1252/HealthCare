using Hangfire;
using HealthCare.Application.Common.Helpers;
using HealthCare.Application.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing.Template;
using System;
using System.Collections.Generic;
using System.Text;
using static System.Net.WebRequestMethods;

namespace HealthCare.Infrastructure.Services;

public class NotificationService(IEmailService emailService, IWebHostEnvironment env) : INotificationService
{
    private readonly IEmailService _emailService = emailService;
    private readonly IWebHostEnvironment _env = env;

    public async Task SendNewAppointmentNotificationAsync(string recipientEmail, string recipientName, string recipientType, string patientName,
        string appointmentDate, string serviceName)
    {
        var template = Path.Combine(_env.WebRootPath, "EmailTemplates", "NewAppointmentNotification.html");

        var data = new Dictionary<string, string>
        {
            { "{{recipientType}}", recipientType },
            { "{{recipientName}}", recipientName },
            { "{{patientName}}", patientName },
            { "{{serviceName}}", serviceName },
            { "{{appointmentDate}}", appointmentDate }
        };

        var emailBody = EmailBodyBuilder.BuildEmailBody(template, data);

        var subject = $"New {recipientType} Task: {serviceName}";

        BackgroundJob.Enqueue(() => _emailService.SendEmailAsync(recipientEmail, subject, emailBody));
        await Task.CompletedTask;
    }
}
