using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HealthCare.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UsersController(ISender mediatr) : ControllerBase
{
    private readonly ISender _mediatr = mediatr;
}
