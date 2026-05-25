using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using InternManagement.Application.DTOs;
using InternManagement.Application.Services;

namespace InternManagement.API.Hubs;

public class ChatHub : Hub
{
    private static readonly Dictionary<string, List<string>> _userConnections = new();
    private readonly ICommunicationService _communicationService;

    public ChatHub(ICommunicationService communicationService)
    {
        _communicationService = communicationService;
    }

    public override async Task OnConnectedAsync()
    {
        var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
                     ?? Context.User?.FindFirst("sub")?.Value;

        if (!string.IsNullOrEmpty(userId))
        {
            var connectionId = Context.ConnectionId;
            if (!_userConnections.ContainsKey(userId))
                _userConnections[userId] = new List<string>();
            _userConnections[userId].Add(connectionId);

            await Groups.AddToGroupAsync(connectionId, $"user_{userId}");
            Console.WriteLine($"[ChatHub] User {userId} connected with connection {connectionId}");
        }

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
                     ?? Context.User?.FindFirst("sub")?.Value;

        if (!string.IsNullOrEmpty(userId))
        {
            var connectionId = Context.ConnectionId;
            if (_userConnections.ContainsKey(userId))
                _userConnections[userId].Remove(connectionId);
            Console.WriteLine($"[ChatHub] User {userId} disconnected");
        }

        await base.OnDisconnectedAsync(exception);
    }

    public async Task SendMessage(string receiverId, string content)
    {
        var senderId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
                       ?? Context.User?.FindFirst("sub")?.Value;

        if (string.IsNullOrEmpty(senderId) || string.IsNullOrEmpty(receiverId))
            return;

        var senderIdInt = int.Parse(senderId);
        var receiverIdInt = int.Parse(receiverId);

        // Save message to database
        var savedMessage = await _communicationService.CreateAsync(new CreateCommunicationRequest(
            SenderId: senderIdInt,
            ReceiverId: receiverIdInt,
            Subject: null,
            MessageContent: content,
            ParentMessageId: null
        ));

        if (savedMessage == null) return;

        // Send to receiver via SignalR
        await Clients.Group($"user_{receiverId}").SendAsync("ReceiveMessage", savedMessage);

        // Confirm to sender
        await Clients.Caller.SendAsync("MessageSent", savedMessage);
    }

    public async Task SendTyping(string receiverId, bool isTyping)
    {
        var senderId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
                       ?? Context.User?.FindFirst("sub")?.Value;

        if (string.IsNullOrEmpty(senderId) || string.IsNullOrEmpty(receiverId))
            return;

        await Clients.Group($"user_{receiverId}").SendAsync("UserTyping", new
        {
            SenderId = int.Parse(senderId),
            IsTyping = isTyping
        });
    }

    public async Task MarkAsRead(string senderId)
    {
        var receiverId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
                         ?? Context.User?.FindFirst("sub")?.Value;

        if (string.IsNullOrEmpty(receiverId) || string.IsNullOrEmpty(senderId))
            return;

        await Clients.Group($"user_{senderId}").SendAsync("MessagesRead", new
        {
            ReaderId = int.Parse(receiverId),
            ReadAt = DateTime.UtcNow
        });
    }
}
