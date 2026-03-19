using HealthCare.Api.Extentions;
using HealthCare.Application.Common.Consts;
using HealthCare.Application.Features.DoctorSlots.Commands.DeleteSlotsById;
using HealthCare.Application.Features.DoctorSlots.Commands.GenerateSlots;
using HealthCare.Application.Features.DoctorSlots.Contracts;
using HealthCare.Application.Features.NurseShifts.Commands.DeleteShiftById;
using HealthCare.Application.Features.NurseShifts.Commands.GenereateShifts;
using HealthCare.Application.Features.NurseShifts.Contracts;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HealthCare.Api.Controllers;

[Route("api/nurses/me/shifts")]
[ApiController]
[Authorize(Roles = DefaultRoles.Nurse)]
public class NurseShiftsController(ISender mediatr) : ControllerBase
{
    private readonly ISender _mediatr = mediatr;

    [HttpPost()]
    public async Task<IActionResult> GenerateShifts([FromBody] GenerateNurseShiftsRequest request, CancellationToken cancellationToken)
    {
        var command = request.Adapt<GenerateNurseShiftsCommand>() with { UserId = User.GetUserId()! };

        var result = await _mediatr.Send(command, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteShiftById([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var command = new DeleteShiftByIdCommand(User.GetUserId()!, id);

        var result = await _mediatr.Send(command, cancellationToken);
        return result.IsSuccess ? NoContent() : result.ToProblem();
    }
}
