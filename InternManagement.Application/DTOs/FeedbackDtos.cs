using InternManagement.Domain.Entities;

namespace InternManagement.Application.DTOs;

public record FeedbackDto(
    int FeedbackId,
    int InternId,
    string FeedbackType,
    int? RelatedId,
    int? Rating,
    string? Comments,
    bool IsAnonymous,
    DateTime SubmittedAt);

public record FeedbackDetailDto(
    int FeedbackId,
    int InternId,
    string? InternName,
    string FeedbackType,
    int? RelatedId,
    string? RelatedType,
    int? Rating,
    string? Comments,
    bool IsAnonymous,
    DateTime SubmittedAt);

public record CreateFeedbackRequest(
    int InternId,
    string FeedbackType,
    int? RelatedId,
    int? Rating,
    string? Comments,
    bool IsAnonymous);

public record UpdateFeedbackRequest(
    int? Rating,
    string? Comments);

public record FeedbackFilter(
    string? FeedbackType,
    int? InternId,
    bool? IsAnonymous,
    DateTime? FromDate,
    DateTime? ToDate);

public static class FeedbackDtoExtensions
{
    public static FeedbackDto ToDto(this Feedback entity) => new(
        entity.FeedbackId, entity.InternId, entity.FeedbackType, entity.RelatedId,
        entity.Rating, entity.Comments, entity.IsAnonymous, entity.SubmittedAt);

    public static Feedback ToEntity(this CreateFeedbackRequest dto) => new()
    {
        InternId = dto.InternId, FeedbackType = dto.FeedbackType, RelatedId = dto.RelatedId,
        Rating = dto.Rating, Comments = dto.Comments, IsAnonymous = dto.IsAnonymous
    };
}
