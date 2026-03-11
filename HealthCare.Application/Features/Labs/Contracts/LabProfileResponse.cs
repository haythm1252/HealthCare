using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Features.Labs.Contracts
{
    public record LabProfileResponse(
        string Id,
        string Name,
        string Email,
        string PhoneNumber,
        string Address,
        string? AddressUrl,
        string City,
        string Bio,
        string ProfilePictureUrl
    ) : IRequest<LabProfileResponse>;
}
