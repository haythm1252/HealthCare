using HealthCare.Api.Extentions;
using HealthCare.Application.Common.Consts;
using HealthCare.Application.Features.Nurse.Quries.NurseProfile;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HealthCare.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class NursesController(ISender mediatr) : ControllerBase
{
    private readonly ISender _mediatr = mediatr;

    [Authorize(Roles = DefaultRoles.Nurse)]
    [HttpGet("profile")]
    public async Task<IActionResult> GetNurseProfile(CancellationToken cancellationToken)
    {
        var res = await _mediatr.Send(new NurseProfleQuery(User.GetUserId()!), cancellationToken);

        return res.IsSuccess ? Ok(res.Value) : res.ToProblem();
    }
}