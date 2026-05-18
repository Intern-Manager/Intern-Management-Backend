using InternManagement.API.Hubs;
using InternManagement.Application.DTOs;
using InternManagement.Application.Repositories;
using Microsoft.AspNetCore.SignalR;

namespace InternManagement.API.Services;

public class NotificationService
{
    private readonly IHubContext<NotificationHub> _hubContext;
    private readonly INotificationRepository _repository;

    public NotificationService(IHubContext<NotificationHub> hubContext, INotificationRepository repository)
    {
        _hubContext = hubContext;
        _repository = repository;
    }

    public async Task SendToUserAsync(int userId, string title, string message, string category = "System", string? relatedType = null, int? relatedId = null)
    {
        // Save to database
        var notification = new Domain.Entities.Notification
        {
            UserId = userId,
            Subject = title,
            Content = message,
            Category = category,
            RelatedType = relatedType,
            RelatedId = relatedId,
            IsRead = false,
            SentAt = DateTime.UtcNow
        };
        await _repository.AddAsync(notification, CancellationToken.None);

        // Send via SignalR
        await _hubContext.Clients.Group($"user_{userId}").SendAsync("ReceiveNotification", new
        {
            NotificationId = notification.NotificationId,
            Title = title,
            Message = message,
            Category = category,
            Timestamp = DateTime.UtcNow
        });
    }

    public async Task SendToRoleAsync(int roleId, string title, string message, string category = "System")
    {
        // Get all users with this role and send to each
        var notifications = new List<Domain.Entities.Notification>();
        
        // Broadcast via SignalR to role group
        await _hubContext.Clients.Group($"role_{roleId}").SendAsync("ReceiveNotification", new
        {
            Title = title,
            Message = message,
            Category = category,
            Timestamp = DateTime.UtcNow
        });
    }

    public async Task BroadcastAsync(string title, string message, string category = "System")
    {
        await _hubContext.Clients.All.SendAsync("ReceiveNotification", new
        {
            Title = title,
            Message = message,
            Category = category,
            Timestamp = DateTime.UtcNow
        });
    }
}
