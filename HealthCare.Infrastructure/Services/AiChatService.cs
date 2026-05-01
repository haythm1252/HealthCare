using Google.GenAI;
using Google.GenAI.Types;
using HealthCare.Application.Common.Settings;
using HealthCare.Application.Features.AiChatBot.Contracts;
using HealthCare.Application.Services;
using HealthCare.Domain.Entities;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Org.BouncyCastle.Ocsp;
using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

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


    public async Task<GeminiParsedResponse> GetModelResponseAsync(string userMessage, string specialtiesList)
    {
        var httpClient = new HttpClient();

        var url = "https://router.huggingface.co/v1/chat/completions";
        var systemPrompt = _aiSettings.SecondarySystemPrompt + specialtiesList + "Dont Forget To Give Initial Diagnosis Or Asking Addtional Question If You Want";

        var requestData = new
        {
            model = "mistralai/Mistral-7B-Instruct-v0.2:featherless-ai",
            max_tokens = 2800,
            messages = new[]
            {
                new { role = "system", content = systemPrompt },
                new { role = "user", content = userMessage }
            }
        };

        httpClient.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _aiSettings.HuggingFaceKey);

        var response = await httpClient.PostAsJsonAsync(url, requestData);


        GeminiParsedResponse Response = new GeminiParsedResponse(
            Message: "I'm sorry, the AI service is currently facing some problems. Please try again",
            SuggestedSpecialty: null
        );

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogWarning("model faild with status code: {StatusCode} and reason: {ReasonPhrase}", response.StatusCode, response.ReasonPhrase);
            return Response;
        }

        try
        {
            using var doc = await JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync());

            var rawJson = doc.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString();

            if (!string.IsNullOrWhiteSpace(rawJson))
            {
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                Response = JsonSerializer.Deserialize<GeminiParsedResponse>(rawJson, options) ?? Response;
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning("Failed to parse model response: {Error}", ex.Message);
        }

        return Response;
    }


    public async Task<GeminiParsedResponse> GetOlamaMedGemmaResponseAsync(string userMessage, string specialtiesList)
    {
        var httpClient = new HttpClient();
        var defaultResponse = new GeminiParsedResponse(
            Message: "I'm sorry, the AI service is currently facing some problems.",
            SuggestedSpecialty: null
        );

        httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0");
        httpClient.DefaultRequestHeaders.Add("ngrok-skip-browser-warning", "true");

        try
        {
            var response = await httpClient.PostAsJsonAsync(_aiSettings.OlamaUrl, new
            {
                model = "medgemma:4b",
                messages = new[] {
                new { role = "system", content = _aiSettings.MedGemmaSystemPrompt + specialtiesList },
                new { role = "user", content = userMessage }
            },
                stream = false
            });

            if (!response.IsSuccessStatusCode) return defaultResponse;
            _logger.LogWarning("MedGemma response success with status code: {StatusCode} and reason: {ReasonPhrase}", response.StatusCode, response.ReasonPhrase);

            using var doc = await JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync());

            var rawContent = doc.RootElement.GetProperty("message").GetProperty("content").GetString() ?? "";

            string cleanJson = rawContent;

            if (rawContent.Contains("{") && rawContent.Contains("}"))
            {
                int startIndex = rawContent.IndexOf("{");
                int endIndex = rawContent.LastIndexOf("}");
                cleanJson = rawContent.Substring(startIndex, (endIndex - startIndex) + 1);
            }

            try
            {
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var result = JsonSerializer.Deserialize<GeminiParsedResponse>(cleanJson, options);
                return result ?? defaultResponse;
            }
            catch
            {
                return new GeminiParsedResponse(Message: rawContent, SuggestedSpecialty: null);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("MedGemma Error: {Error}", ex.Message);
            return defaultResponse;
        }
    }

    
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

