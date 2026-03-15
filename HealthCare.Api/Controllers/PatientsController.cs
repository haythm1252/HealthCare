using HealthCare.Api.Extentions;
using HealthCare.Application.Common.Consts;
using HealthCare.Application.Features.Labs.Commands.UpdateProfile;
using HealthCare.Application.Features.Labs.Contracts;
using HealthCare.Application.Features.Patients.Commands.UpdateProfile;
using HealthCare.Application.Features.Patients.Contracts;
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

    [Authorize(Roles = DefaultRoles.Patient)]
    [HttpPut("profile")]
    public async Task<IActionResult> UpdatePatientsProfile([FromForm] UpdatePatientProfileRequest request, CancellationToken cancellationToken)
    {
        var command = new UpdatePatientProfileCommand(
                User.GetUserId()!,
                request.Name,
                request.PhoneNumber,
                request.Address,
                request.AddressUrl,
                request.City,
                request.Weight);

        var result = await _mediatr.Send(command, cancellationToken);
        return result.IsSuccess ? NoContent() : result.ToProblem();
    }
}
