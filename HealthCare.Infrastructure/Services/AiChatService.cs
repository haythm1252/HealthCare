using Google.GenAI;
using Google.GenAI.Types;
using HealthCare.Application.Common.Settings;
using HealthCare.Application.Services;
using HealthCare.Domain.Entities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Org.BouncyCastle.Ocsp;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Infrastructure.Services;

public class AiChatService(IOptions<AiSettings> aiSettings, ILogger<AiChatService> logger) : IAiChatService
{
    private readonly AiSettings _aiSettings = aiSettings.Value;
    private readonly ILogger<AiChatService> _logger = logger;

    public async Task<string> GetGeminiResponseAsync(string? userMessage, string? attachmentUrl, List<AiMessage> history, string specialtiesList)
    {
        var client = new Client(apiKey: _aiSettings.ApiKey);

        var contents = PrepareContents(userMessage, attachmentUrl, history);
        var config = PrepareConfig(specialtiesList);


        foreach (var modelName in _aiSettings.FallbackModels)
        {
            try
            {
                _logger.LogInformation("Attempting AI request with model: {ModelName}", modelName);

                var response = await client.Models.GenerateContentAsync(
                    model: modelName,
                    contents: contents,
                    config: config
                );

                if (!string.IsNullOrEmpty(response.Text))
                {
                    _logger.LogInformation("Success with model: {ModelName}", modelName);
                    return response.Text;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Model {ModelName} failed: {Error}", modelName, ex.Message);
                continue;
            }
        }

        var errorJson = "{\"Message\": \"I'm sorry, the AI service is currently overloaded. Please try again in a moment.\", \"SuggestedSpecialty\": null}";
        _logger.LogWarning("All models failed. Returning static fallback JSON.");
        return errorJson;
    }



    // convert the history messages and the new message to parts and contents that the gemini api use
    private List<Content> PrepareContents(string? userMessage, string? attachmentUrl, List<AiMessage> history)
    {
        var contents = new List<Content>();

        foreach (var m in history)
        {
            contents.Add(new Content
            {
                Role = m.Role.ToLower(),
                Parts = MapMessageToParts(m.Content, m.AttachmentUrl)
            });
        }

        var currentParts = new List<Part> { new Part { Text = userMessage ?? "" } };

        if (!string.IsNullOrEmpty(attachmentUrl))
        {
            currentParts.Add(new Part { FileData = new FileData { FileUri = attachmentUrl } });
        }

        contents.Add(new Content { Role = "user", Parts = currentParts });
        return contents;
    }

    // make the config (system prompt and the available specialties)
    private GenerateContentConfig PrepareConfig(string specialtiesList)
    {
        return new GenerateContentConfig
        {
            SystemInstruction = new Content
            {
                Parts = new List<Part>
            {
                new Part { Text = $"{_aiSettings.SystemPrompt}\nAvailable specialties: {specialtiesList}" }
            }
            },
            ResponseMimeType = "application/json"
        };
    }
    
    // convert one message to parts the message itself and the file url if exists
    private List<Part> MapMessageToParts(string text, string? url)
    {
        var parts = new List<Part>
            {
                new Part { Text = text }
            };

        if (!string.IsNullOrEmpty(url))
        {
            parts.Add(new Part
            {
                FileData = new FileData
                {
                    FileUri = url
                }
            });
        }

        return parts;
    }
}

