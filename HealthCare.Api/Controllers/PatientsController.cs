using HealthCare.Api.Extentions;
using HealthCare.Application.Common.Consts;
using HealthCare.Application.Features.Patients.Queries.PatientProfile;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HealthCare.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PatientsController(ISender mediatr) : ControllerBase
{
    private readonly ISender _mediatr = mediatr;


    [Authorize(Roles = DefaultRoles.Patient)]
    [HttpGet("profile")]
    public async Task<IActionResult> GetPatientProfile(CancellationToken cancellationToken)
    {
        var res = await _mediatr.Send(new PatientProfileQuery(User.GetUserId()!), cancellationToken);
        return res.IsSuccess ? Ok(res.Value) : res.ToProblem();
    }
}
