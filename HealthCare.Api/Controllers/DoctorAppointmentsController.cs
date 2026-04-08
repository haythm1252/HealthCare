using HealthCare.Api.Extentions;
using HealthCare.Application.Common.Consts;
using HealthCare.Application.Features.Appointments.Contracts;
using HealthCare.Application.Features.DoctorAppointments.Commands.AddDiagnosis;
using HealthCare.Application.Features.DoctorAppointments.Commands.BookDoctorAppointment;
using HealthCare.Application.Features.DoctorAppointments.Contracts;
using HealthCare.Application.Features.DoctorAppointments.Queries.GetDoctorAppointments;
using HealthCare.Application.Features.DoctorAppointments.Queries.PaymentStatus;
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

    [HttpGet("me")]
    [Authorize(Roles = DefaultRoles.Doctor)]
    public async Task<IActionResult> GetMyAppointments([FromQuery] GetAppointmentsRequest request ,CancellationToken cancellationToken)
    {
        var query = request.Adapt<GetDoctorAppointmentsQuery>() with { UserId = User.GetUserId()! };

        var result = await _mediatr.Send(query, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }


    [HttpPost]
    [Authorize(Roles = DefaultRoles.Patient)]
    public async Task<IActionResult> BookAppointment([FromBody] BookDoctorAppointmentRequest request, CancellationToken cancellationToken)
    {
        var command = request.Adapt<BookDoctorAppointmentCommand>() with { UserId = User.GetUserId()! };

        var result = await _mediatr.Send(command, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpPost("{id:guid}/diagnoses")]
    [Authorize(Roles = DefaultRoles.Doctor)]
    public async Task<IActionResult> AddDiagnosis([FromRoute] Guid id, [FromBody] AddDiagnosisRequest request, CancellationToken cancellationToken)
    {
        var command = request.Adapt<AddDiagnosisCommand>() with { UserId = User.GetUserId()!, AppointmentId = id };

        var result = await _mediatr.Send(command, cancellationToken);
        return result.IsSuccess ? NoContent() : result.ToProblem();
    }



    [HttpGet("{id:guid}/payment-status")]
    [Authorize(Roles = DefaultRoles.Patient)]
    public async Task<IActionResult> AppointmentPaymentStatus([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var query = new DoctorAppointmentPaymentStatusQuery(User.GetUserId()!, id);

        var result = await _mediatr.Send(query, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

}
