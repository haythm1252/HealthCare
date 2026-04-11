using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace HealthCare.Application.Features.AiChatBot.Contracts;

public record GeminiParsedResponse(
        [property: JsonPropertyName("message")] string Message,
        [property: JsonPropertyName("suggestedSpecialty")] string? SuggestedSpecialty
);
