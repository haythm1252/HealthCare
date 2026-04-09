using HealthCare.Api.Extentions;
using HealthCare.Application.Common.Consts;
using HealthCare.Application.Features.Appointments.Contracts;
using HealthCare.Application.Features.NurseAppointments.Command.BookNurseAppointment;
using HealthCare.Application.Features.NurseAppointments.Contracts;
using HealthCare.Application.Features.NurseAppointments.Queries.GetNurseAppointmentDetails;
using HealthCare.Application.Features.NurseAppointments.Queries.GetNurseAppointments;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HealthCare.Api.Controllers;

[Route("api/nurse-appointments")]
[ApiController]
public class NurseAppointmentsController(ISender mediatr) : ControllerBase
{
    private readonly ISender _mediatr = mediatr;


    [HttpGet("me")]
    [Authorize(Roles = DefaultRoles.Nurse)]
    public async Task<IActionResult> GetMyAppointments([FromQuery] GetAppointmentsRequest request, CancellationToken cancellationToken)
    {
        var query = request.Adapt<GetNurseAppointmentsQuery>() with { UserId = User.GetUserId()! };

        var result = await _mediatr.Send(query, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpGet("{id:guid}")]
    [Authorize(Roles = $"{DefaultRoles.Patient},{DefaultRoles.Nurse}")]
    public async Task<IActionResult> GetAppointmentDetails([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var query = new GetNurseAppointmentDetailsQuery(User.GetUserId()!, User.GetRole()!, id);

        var result = await _mediatr.Send(query, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpPost]
    [Authorize(Roles = DefaultRoles.Patient)]
    public async Task<IActionResult> BookAppointment([FromBody] BookNurseAppointmentRequest request, CancellationToken cancellationToken)
    {
        var command = request.Adapt<BookNurseAppointmentCommand>() with { UserId = User.GetUserId()! };

        var result = await _mediatr.Send(command, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }
}
