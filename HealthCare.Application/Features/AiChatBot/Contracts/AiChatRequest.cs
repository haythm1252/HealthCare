using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.AiChatBot.Contracts;

public record AiChatRequest(
    IFormFile? Attachment,
    string Message = ""
);
