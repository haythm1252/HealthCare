using HealthCare.Application.Features.Payment.Commands.PaymentCallback;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace HealthCare.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PaymentController(ISender mediatr, IWebHostEnvironment env) : ControllerBase
{
    private readonly ISender _mediatr = mediatr;
    private readonly IWebHostEnvironment _env = env;

    [HttpGet("redirect")]
    public async Task<IActionResult> RedirectCallback()
    {
        //var success = Request.Query["success"].ToString(); 
        //var appointmentId = Request.Query["merchant_order_id"].ToString();
        //var transactionId = Request.Query["id"].ToString();

        //var filePath = Path.Combine(_env.WebRootPath, "EmailTemplates", "PaymentSuccessNotification.html");

        return Redirect("https://unalterably-unasphalted-felton.ngrok-free.dev/EmailTemplates/PaymentSuccessNotification.html");
    }

    [HttpPost("callback")]
    public async Task<IActionResult> Webhook([FromBody] JsonElement payload)
    {
        var signature = Request.Query["hmac"];

        var result = await _mediatr.Send(new PaymentCallbackCommand(payload, signature!));
        return result.IsSuccess ? Ok() : BadRequest();
    }
}
