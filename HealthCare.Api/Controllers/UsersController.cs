using HealthCare.Api.Extentions;
using HealthCare.Application.Common.Consts;
using HealthCare.Application.Features.Users.Commands.BlockUser;
using HealthCare.Application.Features.Users.Commands.MedicalStaffRegister;
using HealthCare.Application.Features.Users.Queries.GetUsers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HealthCare.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UsersController(ISender mediatr) : ControllerBase
{
    private readonly ISender _mediatr = mediatr;

    [Authorize(Roles = DefaultRoles.Admin)]
    [HttpGet]
    public async Task<IActionResult> GetUsers([FromQuery] GetUsersQuery query, CancellationToken cancellationToken)
    {
        var result = await _mediatr.Send(query, cancellationToken);
        return Ok(result);
    }

    [Authorize(Roles = DefaultRoles.Admin)]
    [HttpPatch("{id}/toggle-status")]
    public async Task<IActionResult> ToggleUserStatus([FromRoute] string id, CancellationToken cancellationToken)
    {
        var result = await _mediatr.Send(new ToggleUserStatusCommand(id), cancellationToken);
        return result.IsSuccess ? Ok() : result.ToProblem();
    }

    [Authorize(Roles = DefaultRoles.Admin)]
    [HttpPost("medical-staff-registeration")]
    public async Task<IActionResult> MedicalStaffRegister([FromBody] MedicalStaffRegisterCommand command, CancellationToken cancellationToken)
    {
        var res = await _mediatr.Send(command, cancellationToken);
        return res.IsSuccess ? Ok(res.Value) : res.ToProblem();
    }

}
