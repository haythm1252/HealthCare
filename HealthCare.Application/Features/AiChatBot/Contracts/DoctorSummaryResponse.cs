using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.AiChatBot.Contracts;

public record DoctorSummaryResponse(
    Guid Id,
    string Name,
    string Specialty,
    decimal Rating,
    int RatingCount,
    string City
);
