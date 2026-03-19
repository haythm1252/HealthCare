using HealthCare.Api.Extentions;
using HealthCare.Application.Common.Consts;
using HealthCare.Application.Features.Tests.Commands.AddTest;
using HealthCare.Application.Features.Tests.Commands.DeleteTest;
using HealthCare.Application.Features.Tests.Commands.UpdateTest;
using HealthCare.Application.Features.Tests.Contracts;
using HealthCare.Application.Features.Tests.Queries.GetTests;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HealthCare.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TestsController(ISender mediator) : ControllerBase
{
    private readonly ISender _mediator = mediator;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TestResponse>>> GetAll()
    {
        var result = await _mediator.Send(new GetTestsQuery());
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = DefaultRoles.Admin)]
    public async Task<ActionResult<TestResponse>> Create([FromBody] AddTestCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpPut("{id:Guid}")]
    [Authorize(Roles = DefaultRoles.Admin)]
    public async Task<ActionResult> Update([FromRoute] Guid id, [FromBody] UpdateTestRequest request)
    {
        var command = request.Adapt<UpdateTestCommand>() with { Id = id };
        var result = await _mediator.Send(command);
        return result.IsSuccess ? NoContent() : result.ToProblem();
    }

    [HttpDelete("{id:Guid}")]
    [Authorize(Roles = DefaultRoles.Admin)]
    public async Task<ActionResult> Delete([FromRoute] Guid id)
    {
        var result = await _mediator.Send(new DeleteTestCommand(id));
        return result.IsSuccess ? NoContent() : result.ToProblem();
    }
}