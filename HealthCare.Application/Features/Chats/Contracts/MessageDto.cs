using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.Chats.Contracts;

public record MessageDto(
    Guid Id,
    string SenderId,
    string SenderName,
    string Content,
    DateTime CreatedAt
);