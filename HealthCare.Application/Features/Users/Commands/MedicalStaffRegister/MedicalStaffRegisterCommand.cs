using HealthCare.Application.Common.Result;
using HealthCare.Application.Features.Auth.Contracts;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.Users.Commands.MedicalStaffRegister;

public record MedicalStaffRegisterCommand(
    string Name,
    string? Gender,
    string Email,
    string Address,
    string PhoneNumber,
    string City,
    Guid? SpecialityId,
    string Role

) : IRequest<Result<RegisterResponse>>;
