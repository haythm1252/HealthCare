using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.AiChatBot.Contracts;

public record AiChatResponse(
    string Message,
    string? SuggestedSpecialty,
    IEnumerable<DoctorSummaryResponse>? RecommendedDoctors
);
