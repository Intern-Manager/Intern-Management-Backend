using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace InternManagement.API.Hubs;

public class NotificationHub : Hub
{
    private static readonly Dictionary<string, List<string>> _userConnections = new();

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
            Console.WriteLine($"User {userId} connected with connection {connectionId}");
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
            Console.WriteLine($"User {userId} disconnected");
        }
        
        await base.OnDisconnectedAsync(exception);
    }

    public async Task SendToUser(string userId, string title, string message, object? data = null)
    {
        await Clients.Group($"user_{userId}").SendAsync("ReceiveNotification", new
        {
            Title = title,
            Message = message,
            Data = data,
            Timestamp = DateTime.UtcNow
        });
    }

    public async Task SendToGroup(string groupName, string title, string message, object? data = null)
    {
        await Clients.Group(groupName).SendAsync("ReceiveNotification", new
        {
            Title = title,
            Message = message,
            Data = data,
            Timestamp = DateTime.UtcNow
        });
    }

    public async Task Broadcast(string title, string message, object? data = null)
    {
        await Clients.All.SendAsync("ReceiveNotification", new
        {
            Title = title,
            Message = message,
            Data = data,
            Timestamp = DateTime.UtcNow
        });
    }
}
