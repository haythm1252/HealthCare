using HealthCare.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Services;

public interface IAiChatService
{
    Task<string> GetGeminiResponseAsync(string? userMessage, string? attachmentUrl, List<AiMessage> history, string specialtiesList);
}
