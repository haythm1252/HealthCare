using HealthCare.Api.Extentions;
using HealthCare.Application.Common.Consts;
using HealthCare.Application.Features.Doctors.Queries.GetDoctors;
using HealthCare.Application.Features.Labs.Commands.UpdateProfile;
using HealthCare.Application.Features.Labs.Contracts;
using HealthCare.Application.Features.Labs.Queries.GetLabs;
using HealthCare.Application.Features.Labs.Queries.LabProfile; 
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HealthCare.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class LabsController(ISender mediatr) : ControllerBase
{
    private readonly ISender _mediatr = mediatr;


    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] GetLabsQuery query, CancellationToken cancellationToken)
    {
        var result = await _mediatr.Send(query, cancellationToken);
        return Ok(result);
    }

    [Authorize(Roles = DefaultRoles.Lab)]
    [HttpGet("profile")]
    public async Task<IActionResult> GetLabProfile(CancellationToken cancellationToken)
    {
        var res = await _mediatr.Send(new LabProfileQuery(User.GetUserId()!), cancellationToken);

        return res.IsSuccess ? Ok(res.Value) : res.ToProblem();
    }

    [Authorize(Roles = DefaultRoles.Lab)]
    [HttpPut("profile")]
    public async Task<IActionResult> UpdateLabProfile([FromForm] UpdateLabProfileRequest request, CancellationToken cancellationToken)
    {
        var command = new UpdateLabProfileCommand(
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