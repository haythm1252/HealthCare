using HealthCare.Api.Extentions;
using HealthCare.Application.Common.Consts;
using HealthCare.Application.Features.Chats.Queries.GetChatMessages;
using HealthCare.Application.Features.Chats.Queries.GetChats;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HealthCare.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ChatsController(ISender mediatr) : ControllerBase
{
    private readonly ISender _mediatr = mediatr;

    [HttpGet]
    [Authorize(Roles = $"{DefaultRoles.Patient},{DefaultRoles.Doctor}")]
    public async Task<IActionResult> GetChats()
    {   
        var chats = await _mediatr.Send(new GetChatsQuery(User.GetUserId()!));
        return Ok(chats);
    }

     [HttpGet("{chatId}/messages")]
     [Authorize(Roles = $"{DefaultRoles.Patient},{DefaultRoles.Doctor}")]
     public async Task<IActionResult> GetChatMessages(Guid chatId, int page = 1, int pageSize = 20)
     {
        var result = await _mediatr.Send(new GetChatMessagesQuery(User.GetUserId()!, chatId, page, pageSize));
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
     }
}
