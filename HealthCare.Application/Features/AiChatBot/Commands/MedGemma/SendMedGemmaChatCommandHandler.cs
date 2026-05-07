using HealthCare.Application.Common.Consts;
using HealthCare.Application.Common.Result;
using HealthCare.Application.Errors;
using HealthCare.Application.Features.AiChatBot.Commands.Gemini;
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

namespace HealthCare.Application.Features.AiChatBot.Commands.MedGemma;

public class SendMedGemmaChatCommandHandler(IUnitOfWork unitOfWork,IAiChatService aiService) 
    : IRequestHandler<SendMedGemmaChatCommand, Result<AiChatResponse>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IAiChatService _aiService = aiService;

    public async Task<Result<AiChatResponse>> Handle(SendMedGemmaChatCommand request, CancellationToken cancellationToken)
    {
        var patient = await _unitOfWork.Patients.AsQueryable()
                    .Where(p => p.UserId == request.UserId)
                    .Select(p => new { p.Id, p.User.City })
                    .FirstOrDefaultAsync(cancellationToken);

        if (patient is null)
            return Result.Failure<AiChatResponse>(UserErrors.NotFound);

        var specialties = string.Join(", ", await _unitOfWork.Specialties.AsQueryable().Where(s => !s.IsDeleted)
            .Select(s => s.Name).ToListAsync(cancellationToken));

        // model response
        var aiData = await _aiService.GetMedGemmaResponseAsync(request.Message, specialties);

        // recommendation if the specility not null
        IEnumerable<DoctorSummaryResponse>? recommendedDoctors = null;

        if (!string.IsNullOrWhiteSpace(aiData!.SuggestedSpecialty))
        {
            recommendedDoctors = await _unitOfWork.Doctors.AsQueryable()
                .Where(d => aiData.SuggestedSpecialty.Contains(d.Specialty.Name) || d.Specialty.Name.Contains(aiData.SuggestedSpecialty))
                .OrderByDescending(d => d.User.City == patient.City)
                .ThenByDescending(d => d.Rating)
                .ThenByDescending(d => d.RatingsCount)
                .Take(3)
                .Select(d => new DoctorSummaryResponse(
                    d.Id,
                    d.User.Name,
                    d.Specialty.Name,
                    d.Rating,
                    d.RatingsCount,
                    d.User.City))
                .ToListAsync(cancellationToken);
        }

        var response = new AiChatResponse(aiData!.Message, aiData.SuggestedSpecialty, recommendedDoctors);
        return Result.Success(response);
    }
}
