using HealthCare.Api.Extentions;
using HealthCare.Application.Common.Consts;
using HealthCare.Application.Features.LabAppointment.Commands.BookLabAppointment;
using HealthCare.Application.Features.LabAppointment.Contracts;
using HealthCare.Application.Features.NurseAppointments.Command.BookNurseAppointment;
using HealthCare.Application.Features.NurseAppointments.Contracts;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HealthCare.Api.Controllers;

[Route("api/lab-appointments")]
[ApiController]
public class LabAppointmentsController(ISender mediatr) : ControllerBase
{
    private readonly ISender _mediatr = mediatr;

    [HttpPost]
    [Authorize(Roles = DefaultRoles.Patient)]
    public async Task<IActionResult> BookAppointment([FromBody] BookLabAppointmentRequest request, CancellationToken cancellationToken)
    {
        var command = request.Adapt<BookLabAppointmentCommand>() with { UserId = User.GetUserId()! };

        var result = await _mediatr.Send(command, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }
}
