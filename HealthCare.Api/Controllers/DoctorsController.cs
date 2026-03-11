using HealthCare.Api.Extentions;
using HealthCare.Application.Common.Consts;
using HealthCare.Application.Features.Doctors.Commands.UpdateProfile;
using HealthCare.Application.Features.Doctors.Contracts;
using HealthCare.Application.Features.Doctors.Queries.DoctorProfile;
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
