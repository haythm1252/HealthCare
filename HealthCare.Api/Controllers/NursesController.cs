using HealthCare.Api.Extentions;
using HealthCare.Application.Common.Consts;
using HealthCare.Application.Features.Doctors.Queries.GetDoctors;
using HealthCare.Application.Features.Nurses.Commands.UpdateProfile;
using HealthCare.Application.Features.Nurses.Contracts;
using HealthCare.Application.Features.Nurses.Queries.GetNurses;
using HealthCare.Application.Features.Nurses.Queries.NurseProfile;
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

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] GetNursesQuery query, CancellationToken cancellationToken)
    {
        var result = await _mediatr.Send(query, cancellationToken);
        return Ok(result);
    }

    [Authorize(Roles = DefaultRoles.Nurse)]
    [HttpGet("profile")]
    public async Task<IActionResult> GetNurseProfile(CancellationToken cancellationToken)
    {
        var res = await _mediatr.Send(new NurseProfileQuery(User.GetUserId()!), cancellationToken);
        return res.IsSuccess ? Ok(res.Value) : res.ToProblem();
    }

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
