using HealthCare.Application.Features.Specialities.Queries;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HealthCare.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SpecialtiesController(ISender mediatr) : ControllerBase
{
    private readonly ISender _mediatr = mediatr;

    [HttpGet]
    public async Task<IActionResult> GetSpecialties(CancellationToken cancellationToken)
    {
        var result = await _mediatr.Send(new GetSpecialitiesQuery(), cancellationToken);
        return Ok(result);
    }
}
