using HealthCare.Api.Extentions;
using HealthCare.Application.Common.Consts;
using HealthCare.Application.Features.Doctors.Commands.UpdateConsultationSettings;
using HealthCare.Application.Features.Doctors.Commands.UpdateProfile;
using HealthCare.Application.Features.Doctors.Contracts;
using HealthCare.Application.Features.Doctors.Queries.DoctorBookingDetails;
using HealthCare.Application.Features.Doctors.Queries.DoctorProfile;
using HealthCare.Application.Features.Doctors.Queries.GetDoctors;
using HealthCare.Application.Features.Doctors.Queries.GetSchedule;
using HealthCare.Application.Features.DoctorSlots.Commands.GenerateSlots;
using HealthCare.Application.Features.DoctorSlots.Contracts;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HealthCare.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DoctorsController(ISender mediatr) : ControllerBase
    {
        private readonly ISender _mediatr = mediatr;

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] GetDoctorsQuery query, CancellationToken cancellationToken)
        {
            var result = await _mediatr.Send(query, cancellationToken);
            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetDetails([FromRoute] Guid id, CancellationToken cancellationToken)
        {
            var result = await _mediatr.Send(new GetDoctorBookingDetailsQuery(id), cancellationToken);
            return result.IsSuccess ? Ok(result) : result.ToProblem();
        }

        [Authorize(Roles = DefaultRoles.Doctor)]
        [HttpGet("me/schedule")]
        public async Task<IActionResult> GetSchedule(CancellationToken cancellationToken)
        {
            var result = await _mediatr.Send(new DoctorScheduleQuery(User.GetUserId()!), cancellationToken);
            return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
        }

        [Authorize(Roles = DefaultRoles.Doctor)]
        [HttpPut("me/consultation-settings")]
        public async Task<IActionResult> UpdateConsultationSettings([FromBody] DoctorConsultationSettingsRequest request,CancellationToken cancellationToken)
        {
            var command = request.Adapt<UpdateDoctorConsultationSettingsCommand>() with 
            {
                UserId = User.GetUserId()! 
            };

            var result = await _mediatr.Send(command, cancellationToken);
            return result.IsSuccess ? NoContent() : result.ToProblem();
        } 


        [Authorize(Roles = DefaultRoles.Doctor)]
        [HttpGet("profile")]
        public async Task<IActionResult> GetDoctorProfile(CancellationToken cancellationToken)
        {
            var res = await _mediatr.Send(new DoctorProfileQueries(User.GetUserId()!), cancellationToken);
            return res.IsSuccess ? Ok(res.Value) : res.ToProblem();
        }

        [Authorize(Roles = DefaultRoles.Doctor)]
        [HttpPut("profile")]
        public async Task<IActionResult> UpdateDoctorProfile([FromForm] UpdateDoctorProfileRequest request, CancellationToken cancellationToken)
        {
            var command = new UpdateDoctorProfileCommand(
                    User.GetUserId()!,
                    request.Name,
                    request.Title,
                    request.Bio,
                    request.PhoneNumber,
                    request.Address,
                    request.AddressUrl,
                    request.City,
                    request.ProfilePicture);

            var result = await _mediatr.Send(command, cancellationToken);
            return result.IsSuccess ? NoContent() : result.ToProblem();
        }
    }
}
