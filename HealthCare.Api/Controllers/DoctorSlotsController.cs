using HealthCare.Api.Extentions;
using HealthCare.Application.Common.Consts;
using HealthCare.Application.Features.DoctorSlots.Commands.DeleteSlots;
using HealthCare.Application.Features.DoctorSlots.Commands.DeleteSlotsById;
using HealthCare.Application.Features.DoctorSlots.Commands.GenerateSlots;
using HealthCare.Application.Features.DoctorSlots.Contracts;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace HealthCare.Api.Controllers;

[Route("api/doctors/me/slots")]
[ApiController]
[Authorize(Roles = DefaultRoles.Doctor)]
public class DoctorSlotsController(ISender mediatr) : ControllerBase
{
    private readonly ISender _mediatr = mediatr;

    [HttpPost()]
    public async Task<IActionResult> GenerateSlots([FromBody] GenerateDoctorSlotsRequest request, CancellationToken cancellationToken)
    {
        var command = request.Adapt<GenerateDoctorSlotsCommand>() with { UserId = User.GetUserId()! };

        var result = await _mediatr.Send(command, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpDelete()]
    public async Task<IActionResult> DeleteSlots([FromQuery] DateOnly date, CancellationToken cancellationToken)
    {
        var command = new DeleteSlotsCommand(User.GetUserId()!, date);

        var result = await _mediatr.Send(command, cancellationToken);
        return result.IsSuccess ? NoContent() : result.ToProblem();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteSlotById([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var command = new DeleteSlotByIdCommand(User.GetUserId()!, id);

        var result = await _mediatr.Send(command, cancellationToken);
        return result.IsSuccess ? NoContent() : result.ToProblem();
    }
}
