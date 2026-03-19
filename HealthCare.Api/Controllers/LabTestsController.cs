using HealthCare.Api.Extentions;
using HealthCare.Application.Common.Consts;
using HealthCare.Application.Features.LabTest.Commands.AddLabTest;
using HealthCare.Application.Features.LabTest.Commands.DeleteLabTest;
using HealthCare.Application.Features.LabTest.Commands.UpdateLabTest;
using HealthCare.Application.Features.LabTest.Contracts;
using HealthCare.Application.Features.LabTest.Queries.GetLabTests;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HealthCare.Api.Controllers;

[Route("api/labs/me/[controller]")]
[ApiController]
[Authorize(Roles = DefaultRoles.Lab)]
public class LabTestsController(ISender mediatr) : ControllerBase
{
    private readonly ISender _mediatr = mediatr;

    [HttpGet]
    public async Task<IActionResult> GetMyTests(CancellationToken cancellationToken)
    {
        var result = await _mediatr.Send(new GetLabTestsQuery(User.GetUserId()!), cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] AddLabTestRequest request, CancellationToken cancellationToken)
    {
        var command = request.Adapt<AddLabTestCommand>() with { UserId = User.GetUserId()! };

        var result = await _mediatr.Send(command, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateLabTestRequest request, CancellationToken cancellationToken)
    {
        var command = new UpdateLabTestCommand(
            User.GetUserId()!,
            id,
            request.Price,
            request.IsAvailableAtHome);

        var result = await _mediatr.Send(command, cancellationToken);
        return result.IsSuccess ? Ok() : BadRequest(result.Error);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediatr.Send(new DeleteLabTestCommand(User.GetUserId()!, id) , cancellationToken);
        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }
}
