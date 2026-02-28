using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Services;

public interface IEmailService
{
    Task SendEmailAsync(string email, string subject, string htmlMessage);
}
