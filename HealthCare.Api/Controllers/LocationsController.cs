using HealthCare.Application.Common.Consts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HealthCare.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class LocationsController : ControllerBase
{
    [HttpGet("governorates")]
    public IActionResult GetGovernorates() => Ok(EgyptGovernorates.All);
}
