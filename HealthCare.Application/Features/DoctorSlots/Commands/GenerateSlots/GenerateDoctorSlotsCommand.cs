using HealthCare.Application.Common.Result;
using HealthCare.Application.Features.DoctorSlots.Contracts;
using MediatR;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace HealthCare.Application.Features.DoctorSlots.Commands.GenerateSlots;

public record GenerateDoctorSlotsCommand(
    string UserId,
    DateOnly StartDate,
    DateOnly? EndDate,
    TimeOnly StartTime,
    TimeOnly? EndTime,
    int ConsultationDurationInminutes = 5
) : IRequest<Result<IEnumerable<DailySlotsDto>>>;
