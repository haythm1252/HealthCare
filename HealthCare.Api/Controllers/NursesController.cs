using HealthCare.Api.Contracts.Nurses;
using HealthCare.Api.Extentions;
using HealthCare.Application.Common.Consts;
using HealthCare.Application.Features.Nurses.Commands.UpdateProfile;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HealthCare.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class NursesController(ISender mediatr) : ControllerBase
{
    private readonly ISender _mediatr = mediatr;

    [Authorize(Roles = DefaultRoles.Nurse)]
    [HttpPut("profile")]
    public async Task<IActionResult> UpdateProfile([FromForm] UpdateNurseProfileRequest request, CancellationToken cancellationToken)
    {
        var command = new UpdateNurseProfileCommand(
                User.GetUserId()!,
                request.Name,
                request.Bio,
                request.PhoneNumber,
                request.Address,
                request.AddressUrl,
                request.City,
                request.ProfilePicture);
            
        var result = await _mediatr.Send(command, cancellationToken);
        return result.IsSuccess ? NoContent() : result.ToProblem();
    }
}
