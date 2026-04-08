using HealthCare.Api.Extentions;
using HealthCare.Application.Common.Consts;
using HealthCare.Application.Features.Appointments.Contracts;
using HealthCare.Application.Features.LabAppointment.Commands.AddTestResult;
using HealthCare.Application.Features.LabAppointment.Commands.BookLabAppointment;
using HealthCare.Application.Features.LabAppointment.Contracts;
using HealthCare.Application.Features.LabAppointment.Queries.GetLabAppointments;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HealthCare.Api.Controllers;

[Route("api/lab-appointments")]
[ApiController]
public class LabAppointmentsController(ISender mediatr) : ControllerBase
{
    private readonly ISender _mediatr = mediatr;

    [HttpGet("me")]
    [Authorize(Roles = DefaultRoles.Lab)]
    public async Task<IActionResult> GetMyAppointments([FromQuery] GetAppointmentsRequest request, CancellationToken cancellationToken)
    {
        var query = request.Adapt<GetLabAppointmentsQuery>() with { UserId = User.GetUserId()! };

        var result = await _mediatr.Send(query, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpPost]
    [Authorize(Roles = DefaultRoles.Patient)]
    public async Task<IActionResult> BookAppointment([FromBody] BookLabAppointmentRequest request, CancellationToken cancellationToken)
    {
        var command = request.Adapt<BookLabAppointmentCommand>() with { UserId = User.GetUserId()! };

        var result = await _mediatr.Send(command, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [Authorize(Roles = DefaultRoles.Lab)]
    [HttpPost("{appointmentId:guid}/test-result/{resultId:guid}")]
    public async Task<IActionResult> AddResult([FromRoute] Guid appointmentId, [FromRoute] Guid resultId,
        [FromForm] AddTestResultRequest request, CancellationToken cancellationToken)
    {
        var command = new AddTestResultCommand(
            User.GetUserId()!,
            appointmentId,
            resultId,
            request.Value,
            request.Summary,
            request.ResultFile
        );

        var result = await _mediatr.Send(command, cancellationToken);
        return result.IsSuccess ? NoContent() : result.ToProblem();
    }
}
