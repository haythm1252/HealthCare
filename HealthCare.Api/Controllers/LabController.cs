using HealthCare.Api.Extentions;
using HealthCare.Application.Common.Consts;
using HealthCare.Application.Features.Lab.Quries.LabProfileQuery; // تأكد من الـ Namespace
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HealthCare.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class LabsController(ISender mediatr) : ControllerBase
{
    private readonly ISender _mediatr = mediatr;

    [Authorize(Roles = DefaultRoles.Lab)] 
    [HttpGet("profile")]
    public async Task<IActionResult> GetLabProfile(CancellationToken cancellationToken)
    {
        var res = await _mediatr.Send(new LabProfileQuery(User.GetUserId()!), cancellationToken);

        return res.IsSuccess ? Ok(res.Value) : res.ToProblem();
    }
}