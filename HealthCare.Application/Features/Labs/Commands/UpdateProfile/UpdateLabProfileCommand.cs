using HealthCare.Application.Common.Result;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.Labs.Commands.UpdateProfile;

public record UpdateLabProfileCommand(
    string UserId,
    string Name,
    string Bio,
    string PhoneNumber,
    string Address,
    string? AddressUrl,
    string City,
    IFormFile? ProfilePicture
) : IRequest<Result>;
