using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.Chats.Contracts;

public record ChatDto(
    Guid Id,
    string Name,
    string? PictureUrl,
    string? LastMessage,
    DateTime? LastMessageAt,
    bool IsLastMessageFromMe = false
);
