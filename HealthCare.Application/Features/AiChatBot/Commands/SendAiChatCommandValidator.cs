using FluentValidation;
using HealthCare.Application.Common.Consts;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.AiChatBot.Commands;

public class SendAiChatCommandValidator : AbstractValidator<SendAiChatCommand>
{
    public SendAiChatCommandValidator()
    {
        RuleFor(x => x.Message)
            .NotNull()
            .MaximumLength(600);

        RuleFor(x => x.Attachment)
            .Must(BeAValidFile!)
            .WithMessage("Only PDF, JPG, JPEG, or PNG files are allowed.")
            .Must(HaveValidSize!)
            .WithMessage("File size exceeds the allowed limit.")
            .When(x => x.Attachment != null);
    }

    private bool BeAValidFile(IFormFile file)
    {
        if (file == null) return false;

        var contentTypes = new List<string> { "application/pdf", "image/jpeg", "image/jpg", "image/png" };
        var extensions = new List<string> { ".pdf", ".jpg", ".jpeg", ".png" };

        var fileExtension = Path.GetExtension(file.FileName).ToLower();

        return contentTypes.Contains(file.ContentType.ToLower()) ||
               extensions.Contains(fileExtension);
    }

    private bool HaveValidSize(IFormFile file)
    {
        if (file == null) return false;

        if (file.ContentType.Equals("application/pdf", StringComparison.OrdinalIgnoreCase))
            return file.Length <= FileSettings.MaxPdfSizeInBytes;

        return file.Length <= FileSettings.MaxImageSizeInBytes;
    }
}
