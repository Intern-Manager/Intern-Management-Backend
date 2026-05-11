using InternManagement.Domain.Entities;

namespace InternManagement.Application.DTOs;

public record CommunicationDto(
    int MessageId,
    int SenderId,
    int ReceiverId,
    string? Subject,
    string MessageContent,
    bool IsRead,
    int? ParentMessageId,
    DateTime SentAt,
    DateTime? ReadAt);

public record CommunicationDetailDto(
    int MessageId,
    int SenderId,
    string SenderName,
    int ReceiverId,
    string ReceiverName,
    string? Subject,
    string MessageContent,
    bool IsRead,
    int? ParentMessageId,
    DateTime SentAt,
    DateTime? ReadAt);

public record CreateCommunicationRequest(
    int SenderId,
    int ReceiverId,
    string? Subject,
    string MessageContent,
    int? ParentMessageId);

public record UpdateCommunicationRequest(
    bool? IsRead,
    DateTime? ReadAt);

public record CommunicationFilter(
    int? SenderId,
    int? ReceiverId,
    bool? IsRead);

public static class CommunicationDtoExtensions
{
    public static CommunicationDto ToDto(this Communication entity) => new(
        entity.MessageId, entity.SenderId, entity.ReceiverId, entity.Subject,
        entity.MessageContent, entity.IsRead, entity.ParentMessageId, entity.SentAt, entity.ReadAt);

    public static Communication ToEntity(this CreateCommunicationRequest dto) => new()
    {
        SenderId = dto.SenderId, ReceiverId = dto.ReceiverId, Subject = dto.Subject,
        MessageContent = dto.MessageContent, ParentMessageId = dto.ParentMessageId
    };
}
