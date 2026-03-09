using HealthCare.Application.Common.Result;
using HealthCare.Application.Features.Auth.Contracts;
using HealthCare.Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.Auth.Commands.RegisterPatient;

public record RegisterPatientCommand
(
    string Name,
    DateOnly DateOfBirth,
    string City,
    string Address,
    string? AddressUrl,
    string PhoneNumber,
    string Gender,
    string Email,
    string Password,
    DiseasesDto Diseases,
    decimal? Weight
) : IRequest<Result<RegisterResponse>>;

