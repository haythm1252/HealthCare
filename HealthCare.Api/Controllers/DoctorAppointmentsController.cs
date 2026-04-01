using HealthCare.Api.Extentions;
using HealthCare.Application.Common.Consts;
using HealthCare.Application.Features.DoctorAppointments.Commands.BookDoctorAppointment;
using HealthCare.Application.Features.DoctorAppointments.Contracts;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HealthCare.Api.Controllers;

[Route("api/doctor-appointments")]
[ApiController]
public class DoctorAppointmentsController(ISender mediatr) : ControllerBase
{
    private readonly ISender _mediatr = mediatr;

    [HttpPost]
    [Authorize(Roles = DefaultRoles.Patient)]
    public async Task<IActionResult> BookAppointment([FromBody] BookDoctorAppointmentRequest request, CancellationToken cancellationToken)
    {
        var command = request.Adapt<BookDoctorAppointmentCommand>() with { UserId = User.GetUserId()! };

        var result = await _mediatr.Send(command, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }
}
