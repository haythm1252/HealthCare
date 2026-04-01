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

    public async Task SendPaymentConfrimationNotificationAsync(string patientEmail, string patientName, string doctorName, DateOnly appointmentDate,
    TimeOnly startTime, TimeOnly endTime, string serviceName)
    {
        var templatePath = Path.Combine(_env.WebRootPath, "EmailTemplates", "PaymentSuccessNotification.html");

        var data = new Dictionary<string, string>
    {
        { "{{patientName}}", patientName },
        { "{{doctorName}}", doctorName },
        { "{{serviceName}}", serviceName },
        { "{{appointmentDate}}", appointmentDate.ToString("dd MMM yyyy") },
        { "{{appointmentTime}}", $"{startTime:hh:mm tt} - {endTime:hh:mm tt}" }
    };

        var emailBody = EmailBodyBuilder.BuildEmailBody(templatePath, data);

        var subject = $"Payment Confirmed: Your appointment with Dr. {doctorName}";

        // Using Hangfire to send the email so the Webhook stays fast
        BackgroundJob.Enqueue(() => _emailService.SendEmailAsync(patientEmail, subject, emailBody));

        await Task.CompletedTask;
    }
}
