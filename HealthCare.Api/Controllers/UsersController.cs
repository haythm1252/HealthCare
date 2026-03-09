using HealthCare.Application.Common.Consts;
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
}
