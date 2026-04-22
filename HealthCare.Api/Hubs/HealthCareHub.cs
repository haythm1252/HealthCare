using HealthCare.Application.Common.Consts;
using HealthCare.Application.Features.Communication.Commands.JoinChat;
using HealthCare.Application.Features.Communication.Commands.JoinVideo;
using HealthCare.Application.Features.Communication.Commands.SendMessage;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace HealthCare.Api.Hubs;

[Authorize(Roles = $"{DefaultRoles.Patient},{DefaultRoles.Doctor}")]
public class HealthCareHub(ISender mediatr, ILogger<HealthCareHub> logger) : Hub
{
    private readonly ISender _mediatr = mediatr;
    private readonly ILogger<HealthCareHub> _logger = logger;

    // join chat room 
    public async Task JoinChat(Guid chatId)
    {
        var userId = Context.UserIdentifier;
        if(string.IsNullOrEmpty(userId))
        {
            await Clients.Caller.SendAsync("Error", "User is not authenticated.");
            return;
        }

        var result = await _mediatr.Send(new JoinChatCommand(userId, chatId));

        if (result.IsSuccess)
        {
            var groupName = $"Chat_{chatId}";
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            _logger.LogInformation("joiend chat");

        }
        else
        {
            await Clients.Caller.SendAsync("Error", result.Error.Description);
        }
    }

    // send message in chat
    public async Task SendMessage(Guid chatId, string content)
    {
        var userId = Context.UserIdentifier;

        if(string.IsNullOrEmpty(userId))
        {
            await Clients.Caller.SendAsync("Error", "User is not authenticated.");
            return;
        }

        var result = await _mediatr.Send(new SendMessageCommand(userId, chatId, content));

        if (result.IsSuccess)
        {
            await Clients.Group($"Chat_{chatId}").SendAsync("ReceiveMessage", result.Value);
            _logger.LogInformation("message sent successfully");
        }
        else
        {
            await Clients.Caller.SendAsync("Error", result.Error.Description);
        }
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await base.OnDisconnectedAsync(exception);
    }
}
