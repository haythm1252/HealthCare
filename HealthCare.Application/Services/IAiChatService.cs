using HealthCare.Application.Features.AiChatBot.Contracts;
using HealthCare.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Services;

public interface IAiChatService
{
    Task<string> GetGeminiResponseAsync(string? userMessage, string? attachmentUrl, List<AiMessage> history, string specialtiesList);
    Task<GeminiParsedResponse> GetModelResponseAsync(string userMessage, string specialtiesList);

    Task<GeminiParsedResponse> GetMedGemmaResponseAsync(string userMessage, string specialtiesList);

}
