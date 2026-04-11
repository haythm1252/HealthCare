using HealthCare.Api.Extentions;
using HealthCare.Application.Common.Consts;
using HealthCare.Application.Features.Specialities.Commands.AddSpeciality;
using HealthCare.Application.Features.Specialities.Commands.DeleteSpeciality;
using HealthCare.Application.Features.Specialities.Commands.UpdateSpeciality;
using HealthCare.Application.Features.Specialities.Contracts;
using HealthCare.Application.Features.Specialities.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HealthCare.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SpecialtiesController(ISender mediatr) : ControllerBase
{
    private readonly ISender _mediatr = mediatr;

    [HttpGet]
    public async Task<IActionResult> GetSpecialties(CancellationToken cancellationToken)
    {
        var result = await _mediatr.Send(new GetSpecialitiesQuery(), cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = DefaultRoles.Admin)]
    public async Task<IActionResult> AddSpecialty([FromBody] SpecialityRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediatr.Send(new AddSpecialityCommand(request.Name), cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = DefaultRoles.Admin)]
    public async Task<IActionResult> UpdateSpecialty([FromRoute] Guid id, [FromBody] SpecialityRequest  request, CancellationToken cancellationToken)
    {
        var result = await _mediatr.Send(new UpdateSpecialityCommand(id, request.Name), cancellationToken);
        return result.IsSuccess ? NoContent() : result.ToProblem();

    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = DefaultRoles.Admin)]
    public async Task<IActionResult> DeleteSpecialties([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediatr.Send(new DeleteSpecialityCommand(id), cancellationToken);
        return result.IsSuccess ? NoContent() : result.ToProblem();
    }

}
