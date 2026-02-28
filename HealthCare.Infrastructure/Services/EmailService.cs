using HealthCare.Application.Common.Settings;
using HealthCare.Application.Services;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Infrastructure.Services;

public class EmailService(IOptions<MailSettings> mailSetting, ILogger<EmailService> logger) : IEmailService
{
    private readonly MailSettings _mailSettings = mailSetting.Value;
    private readonly ILogger<EmailService> _logger = logger;

    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(_mailSettings.DisplayName, _mailSettings.Email));
        message.To.Add(new MailboxAddress("", email));
        message.Subject = subject;
        var bodyBuilder = new BodyBuilder { HtmlBody = htmlMessage };
        message.Body = bodyBuilder.ToMessageBody();

        using var client = new SmtpClient();
        try
        {
            await client.ConnectAsync(_mailSettings.SmtpServer, _mailSettings.Port, SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(_mailSettings.Email, _mailSettings.Password);

            await client.SendAsync(message);
            _logger.LogInformation("Email sent successfully to {email}", email);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {email}", email);
            throw;
        }
        finally
        {
            await client.DisconnectAsync(true);
        }
    }
}
