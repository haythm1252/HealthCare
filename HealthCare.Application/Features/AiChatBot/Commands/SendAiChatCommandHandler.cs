using HealthCare.Application.Common.Consts;
using HealthCare.Application.Common.Result;
using HealthCare.Application.Errors;
using HealthCare.Application.Features.AiChatBot.Contracts;
using HealthCare.Application.Interfaces.Repositories.UnitOfWork;
using HealthCare.Application.Services;
using HealthCare.Domain.Entities;
using HealthCare.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;

namespace HealthCare.Application.Features.AiChatBot.Commands;

public class SendAiChatCommandHandler(
        IUnitOfWork unitOfWork,
        IAiChatService aiService,
        ICloudinaryService cloudinaryService,
        ILogger<SendAiChatCommandHandler> logger
    ) : IRequestHandler<SendAiChatCommand, Result<AiChatResponse>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IAiChatService _aiService = aiService;
    private readonly ICloudinaryService _cloudinaryService = cloudinaryService;
    private readonly ILogger<SendAiChatCommandHandler> _logger = logger;

    public async Task<Result<AiChatResponse>> Handle(SendAiChatCommand request, CancellationToken cancellationToken)
    {
        var patient = await _unitOfWork.Patients.AsQueryable()
            .Where(p => p.UserId == request.UserId)
            .Select(p => new {p.Id, p.User.City})
            .FirstOrDefaultAsync(cancellationToken);

        if (patient is null) 
            return Result.Failure<AiChatResponse>(UserErrors.NotFound);

        // if the request contain files or images
        string? attachmentUrl = null;
        string? publicId = null;
        var contentType = ContentType.Text;

        if (request.Attachment != null)
        {
            string fileType = request.Attachment.ContentType.ToLower();

            using var stream = request.Attachment.OpenReadStream();

            Result<(string Url, string PublicId)> uploadResult;

            if (fileType == "application/pdf")
            {
                uploadResult = await _cloudinaryService.UploadPdfAsync(stream, request.Attachment.FileName);
                contentType = ContentType.File;
            }
            else if (fileType.StartsWith("image/"))
            {
                uploadResult = await _cloudinaryService.UploadImageAsync(stream, request.Attachment.FileName);
                contentType = ContentType.Image;
            }
            else
                return Result.Failure<AiChatResponse>(new Error("File.Type", "Unsupported file type.",400));

            if (uploadResult.IsFailure)
                return Result.Failure<AiChatResponse>(uploadResult.Error);

            attachmentUrl = uploadResult.Value.Url;
            publicId = uploadResult.Value.PublicId;
        }


        // get last messages to make the ai understand the conversation and the specilites so we can recommedn doctors
        var history = await _unitOfWork.AiMessages.AsQueryable()
            .Where(m => m.PatientId == patient.Id)
            .OrderByDescending(m => m.CreatedAt)
            .Take(4).OrderBy(m => m.CreatedAt)
            .ToListAsync(cancellationToken);

        var specialties = string.Join(", ", await _unitOfWork.Specialties.AsQueryable().Select(s => s.Name).ToListAsync(cancellationToken));


        // gemini response
        GeminiParsedResponse? aiData;
        try
        {
            var jsonResponse = await _aiService.GetGeminiResponseAsync(request.Message, attachmentUrl, history, specialties);
            aiData = JsonSerializer.Deserialize<GeminiParsedResponse>(jsonResponse, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
        catch (Exception ex)
        {
            _logger.LogError("Chatbot JSON Error: {Message}", ex.Message);
            return Result.Failure<AiChatResponse>(new Error("Chatbot.Error", "Failed to process AI response", 500));
        }


        // recommendation if the specility not null
        IEnumerable<DoctorSummaryResponse>? recommendedDoctors = null;

        if (!string.IsNullOrWhiteSpace(aiData!.SuggestedSpecialty))
        {
            recommendedDoctors = await _unitOfWork.Doctors.AsQueryable()
                .Where(d => d.Specialty.Name == aiData.SuggestedSpecialty && d.User.City == patient.City)
                .OrderByDescending(d => d.Rating)
                .ThenByDescending(d => d.RatingsCount)
                .Take(3)
                .Select(d => new DoctorSummaryResponse(d.Id, d.User.Name, d.Specialty.Name, d.Rating, d.RatingsCount, d.User.City))
                .ToListAsync(cancellationToken);
        }

        //save the patient and ai message
        var messagesToSave = new List<AiMessage>
        {
            new() {
                Content = request.Message ?? "",
                Role = "user",
                PatientId = patient.Id,
                AttachmentUrl = attachmentUrl,
                ContentType = contentType
            },
            new() {
                Content = aiData!.Message,
                Role = "model",
                PatientId = patient.Id,
                ContentType = ContentType.Text
            }
        };

        await _unitOfWork.AiMessages.AddRangeAsync(messagesToSave, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var response = new AiChatResponse(aiData!.Message, aiData.SuggestedSpecialty, recommendedDoctors);
        return Result.Success(response);
    }
}
