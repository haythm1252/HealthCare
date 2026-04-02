using HealthCare.Api.Extentions;
using HealthCare.Application.Common.Consts;
using HealthCare.Application.Features.Reviews.Commands.AddReview;
using HealthCare.Application.Features.Reviews.Contracts;
using HealthCare.Application.Features.Reviews.Queries.GetReviews;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HealthCare.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ReviewsController(ISender mediatr) : ControllerBase
{
    private readonly ISender mediatr = mediatr;

    [HttpGet]
    public async Task<IActionResult> GetReviews([FromQuery] GetReviewsQuery query, CancellationToken cancellationToken)
    {
        var result = await mediatr.Send(query, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpPost]
    [Authorize(Roles = DefaultRoles.Patient)]
    public async Task<IActionResult> AddReview([FromBody] AddReviewRequest request, CancellationToken cancellationToken)
    {
        var command = request.Adapt<AddReviewCommand>() with { UserId = User.GetUserId()! };

        var result = await mediatr.Send(command, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }
}
