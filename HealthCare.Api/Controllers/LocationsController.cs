using HealthCare.Application.Common.Consts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HealthCare.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class LocationsController : ControllerBase
{
    [HttpGet("cities")]
    public IActionResult GetCities() => Ok(EgyptGovernorates.All);
}
