using HealthCare.Application.Common.Result;
using HealthCare.Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.Patients.Commands.UpdateProfile;

public record UpdatePatientProfileCommand(
    string UserId,
    string Name,
    string PhoneNumber,
    string Address,
    string? AddressUrl,
    string City,
    decimal? Weight
) : IRequest<Result>;
