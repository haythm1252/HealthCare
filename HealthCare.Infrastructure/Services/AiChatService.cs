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

public class AiChatService(IOptions<AiSettings> aiSettings,ILogger<AiChatService> logger) : IAiChatService
{
    private readonly AiSettings _aiSettings = aiSettings.Value;
    private readonly ILogger<AiChatService> _logger = logger;

    public async Task<string> GetGeminiResponseAsync(
    string? userMessage,
    string? attachmentUrl,
    List<AiMessage> history,
    string specialtiesList)
    {
        var client = new Client(apiKey: _aiSettings.ApiKey);

        // Get the old messages from database 
        var contents = new List<Content>();

        foreach (var m in history)
        {
            contents.Add(new Content
            {
                Role = m.Role.ToLower(), 
                Parts = MapMessageToParts(m.Content, m.AttachmentUrl)
            });
        }

        // the new message now
        var currentParts = new List<Part>
            {
                new Part { Text = userMessage ?? "" }
            };

        // add the file url if it exist which we just uploaded to cloudinary in the handler
        if (!string.IsNullOrEmpty(attachmentUrl))
        {
            currentParts.Add(new Part
            {
                FileData = new FileData
                {
                    FileUri = attachmentUrl
                }
            });
        }

        contents.Add(new Content
        {
            Role = "user",
            Parts = currentParts
        });

        // adding the prompt and the specialites to the configureation
        var config = new GenerateContentConfig
        {
            SystemInstruction = new Content
            {
                Parts = new List<Part>
            {
                new Part
                {
                    Text = $"{_aiSettings.SystemPrompt}\nAvailable specialties: {specialtiesList}"
                }
            }
            },
            ResponseMimeType = "application/json"
        };

        string responseText = "I'm sorry, I couldn't process that, please try again.";

        //send the response
        try
        {
            var response = await client.Models.GenerateContentAsync(
                model: "gemini-2.5-flash",
                contents: contents,
                config: config
            );
            responseText = response.Text!;
        }
        catch(Exception ex)
        {
            _logger.Log(LogLevel.Error, ex.Message);
            _logger.Log(LogLevel.Information, responseText);
        }

        _logger.Log(LogLevel.Information, responseText);
        return responseText;
    }

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

