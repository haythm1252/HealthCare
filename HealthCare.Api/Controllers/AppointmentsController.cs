using HealthCare.Api.Extentions;
using HealthCare.Application.Common.Consts;
using HealthCare.Application.Features.Appointments.Commands.AppointmentConfirmation;
using HealthCare.Application.Features.Appointments.Commands.CancelAppointment;
using HealthCare.Application.Features.Appointments.Commands.UpdateFinalStatus;
using HealthCare.Application.Features.Appointments.Contracts;
using HealthCare.Application.Features.Appointments.Queries.PatientAppointmentHistory;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HealthCare.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AppointmentsController(ISender mediatr) : ControllerBase
{
    private readonly ISender _mediatr = mediatr;


    [HttpGet("me/patient-history")]
    [Authorize(Roles = DefaultRoles.Patient)]
    public async Task<IActionResult> GetPatientAppointmentHistory(CancellationToken cancellationToken)
    {
        var query = new PatientAppointmentHistoryQuery(User.GetUserId()!);

        var result = await _mediatr.Send(query, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }


    [HttpPatch("{id:Guid}/confrimation")]
    [Authorize(Roles = $"{DefaultRoles.Doctor},{DefaultRoles.Nurse},{DefaultRoles.Lab}")]
    public async Task<IActionResult> AppointmentConfrimation([FromRoute] Guid id, [FromBody] UpdateAppointmentStatusRequest request, CancellationToken cancellationToken)
    {
        var command = request.Adapt<AppointmentConfrimationCommand>() 
            with { AppointmentId = id, UserId = User.GetUserId()!, UserRole = User.GetRole()! };

        var result = await _mediatr.Send(command, cancellationToken);
        return result.IsSuccess ? NoContent() : result.ToProblem();
    }

    [HttpPatch("{id:Guid}/final-status")]
    [Authorize(Roles = $"{DefaultRoles.Doctor},{DefaultRoles.Nurse},{DefaultRoles.Lab}")]
    public async Task<IActionResult> UpdateFinalStatus([FromRoute] Guid id, [FromBody] UpdateAppointmentStatusRequest request, CancellationToken cancellationToken)
    {
        var command = request.Adapt<UpdateAppointmentFinalStatusCommand>()
            with { AppointmentId = id, UserId = User.GetUserId()!, UserRole = User.GetRole()! };

        var result = await _mediatr.Send(command, cancellationToken);
        return result.IsSuccess ? NoContent() : result.ToProblem();
    }

    [HttpPatch("{id:Guid}/cancellation")]
    [Authorize(Roles = DefaultRoles.Patient)]
    public async Task<IActionResult> CancelAppointment([FromRoute] Guid id, [FromBody] CancelAppointmetnRequest request, CancellationToken cancellationToken)
    {
        var command = request.Adapt<CancelAppointmentCommand>() with { AppointmentId = id, UserId = User.GetUserId()!};

        var result = await _mediatr.Send(command, cancellationToken);
        return result.IsSuccess ? NoContent() : result.ToProblem();
    }
}
