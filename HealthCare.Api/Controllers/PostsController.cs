using HealthCare.Api.Extentions;
using HealthCare.Application.Common.Consts;
using HealthCare.Application.Features.Community.Commands.AddPost;
using HealthCare.Application.Features.Community.Commands.DeletePost;
using HealthCare.Application.Features.Community.Commands.TogglePostStatus;
using HealthCare.Application.Features.Community.Commands.UpdatePost;
using HealthCare.Application.Features.Community.Contracts;
using HealthCare.Application.Features.Community.Queries.GetDoctorPosts;
using HealthCare.Application.Features.Community.Queries.GetPosts;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace HealthCare.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PostsController(ISender mediatr) : ControllerBase
{
    private readonly ISender _mediatr = mediatr;


    [HttpGet()]
    [Authorize(Roles = $"{DefaultRoles.Patient},{DefaultRoles.Admin}")]
    public async Task<IActionResult> GetPosts([FromQuery] GetPostsRequest request, CancellationToken cancellationToken = default)
    {
        var query = request.Adapt<GetPostsQuery>() with { UserRole = User.GetRole()! };

        var result = await _mediatr.Send(query, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpGet("me")]
    [Authorize(Roles = DefaultRoles.Doctor)]
    public async Task<IActionResult> GetDoctorPosts([FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken cancellationToken = default)
    {
        var query = new GetDoctorPostsQuery(User.GetUserId()!, page, pageSize);

        var result = await _mediatr.Send(query, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpPost()]
    [Authorize(Roles = DefaultRoles.Doctor)]
    public async Task<IActionResult> CreatePost([FromForm] AddPostRequest request, CancellationToken cancellationToken)
    {
        var command = new AddPostCommand(
            User.GetUserId()!,
            request.SpecialtyId,
            request.Title,
            request.Content,
            request.AttachmentFile
        );

        var result = await _mediatr.Send(command, cancellationToken);
        return result.IsSuccess ? Created() : result.ToProblem();
    }

    [HttpPut("{postId:guid}")]
    [Authorize(Roles = DefaultRoles.Doctor)]
    public async Task<IActionResult> UpdatePost([FromRoute] Guid postId, [FromForm] UpdatePostRequest request, CancellationToken cancellationToken)
    {
        var command = new UpdatePostCommand(
            User.GetUserId()!,
            postId,
            request.SpecialtyId,
            request.Title,
            request.Content,
            request.AttachmentFile
        );

        var result = await _mediatr.Send(command, cancellationToken);
        return result.IsSuccess ? NoContent() : result.ToProblem();
    }

    [HttpDelete("{postId:guid}")]
    [Authorize(Roles = DefaultRoles.Doctor)]
    public async Task<IActionResult> DeletePost([FromRoute] Guid postId, CancellationToken cancellationToken)
    {
        var command = new DeletePostCommand(postId, User.GetUserId()!);

        var result = await _mediatr.Send(command, cancellationToken);
        return result.IsSuccess ? NoContent() : result.ToProblem();
    }

    [HttpPatch("{postId:guid}/toggle-publish-status")]
    [Authorize(Roles = DefaultRoles.Admin)]
    public async Task<IActionResult> TogglePostStatus([FromRoute] Guid postId, CancellationToken cancellationToken)
    {
        var command = new TogglePostStatusCommand(postId);

        var result = await _mediatr.Send(command, cancellationToken);
        return result.IsSuccess ? NoContent() : result.ToProblem();
    }

}
