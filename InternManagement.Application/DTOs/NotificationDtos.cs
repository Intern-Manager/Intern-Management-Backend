using InternManagement.Domain.Entities;

namespace InternManagement.Application.DTOs;

public record NotificationDto(
    int NotificationId,
    int UserId,
    string NotificationType,
    string Category,
    string Subject,
    string Content,
    int? RelatedId,
    string? RelatedType,
    bool IsRead,
    DateTime SentAt,
    DateTime? ReadAt);

public record NotificationDetailDto(
    int NotificationId,
    int UserId,
    string UserName,
    string NotificationType,
    string Category,
    string Subject,
    string Content,
    int? RelatedId,
    string? RelatedType,
    bool IsRead,
    DateTime SentAt,
    DateTime? ReadAt);

public record CreateNotificationRequest(
    int UserId,
    string NotificationType,
    string Category,
    string Subject,
    string Content,
    int? RelatedId,
    string? RelatedType);

public record UpdateNotificationRequest(
    bool? IsRead,
    DateTime? ReadAt);

public record NotificationFilter(
    string? Category,
    int? UserId,
    bool? IsRead);

public static class NotificationDtoExtensions
{
    public static NotificationDto ToDto(this Notification entity) => new(
        entity.NotificationId, entity.UserId, entity.NotificationType, entity.Category,
        entity.Subject, entity.Content, entity.RelatedId, entity.RelatedType,
        entity.IsRead, entity.SentAt, entity.ReadAt);

    public static Notification ToEntity(this CreateNotificationRequest dto) => new()
    {
        UserId = dto.UserId, NotificationType = dto.NotificationType, Category = dto.Category,
        Subject = dto.Subject, Content = dto.Content, RelatedId = dto.RelatedId,
        RelatedType = dto.RelatedType
    };
}
