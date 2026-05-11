using InternManagement.Domain.Entities;

namespace InternManagement.Application.DTOs;

public record InterviewDto(
    int InterviewId,
    int? CampaignId,
    int? ApplicationId,
    int? InternId,
    int InterviewerId,
    DateTime ScheduledTime,
    int DurationMinutes,
    string InterviewType,
    string? MeetingLink,
    string? Location,
    string Status,
    string? Feedback,
    int? Rating,
    DateTime CreatedAt,
    DateTime? UpdatedAt);

public record InterviewDetailDto(
    int InterviewId,
    int? CampaignId,
    string? CampaignTitle,
    int? ApplicationId,
    int? InternId,
    string? InternName,
    int InterviewerId,
    string? InterviewerName,
    DateTime ScheduledTime,
    int DurationMinutes,
    string InterviewType,
    string? MeetingLink,
    string? Location,
    string Status,
    string? Feedback,
    int? Rating,
    DateTime CreatedAt,
    DateTime? UpdatedAt);

public record CreateInterviewRequest(
    int? CampaignId,
    int? ApplicationId,
    int? InternId,
    int InterviewerId,
    DateTime ScheduledTime,
    int DurationMinutes,
    string InterviewType,
    string? MeetingLink,
    string? Location,
    string? Feedback,
    int? Rating);

public record UpdateInterviewRequest(
    DateTime? ScheduledTime,
    int? DurationMinutes,
    string? InterviewType,
    string? MeetingLink,
    string? Location,
    string? Status,
    string? Feedback,
    int? Rating);

public record InterviewFilter(
    string? Search,
    string? Status,
    int? CampaignId,
    int? InternId,
    int? InterviewerId,
    DateTime? FromDate,
    DateTime? ToDate);

public static class InterviewDtoExtensions
{
    public static InterviewDto ToDto(this Interview entity) => new(
        entity.InterviewId, entity.CampaignId, entity.ApplicationId, entity.InternId,
        entity.InterviewerId, entity.ScheduledTime, entity.DurationMinutes, entity.InterviewType,
        entity.MeetingLink, entity.Location, entity.Status, entity.Feedback, entity.Rating,
        entity.CreatedAt, entity.UpdatedAt);

    public static Interview ToEntity(this CreateInterviewRequest dto) => new()
    {
        CampaignId = dto.CampaignId, ApplicationId = dto.ApplicationId, InternId = dto.InternId,
        InterviewerId = dto.InterviewerId, ScheduledTime = dto.ScheduledTime, DurationMinutes = dto.DurationMinutes,
        InterviewType = dto.InterviewType, MeetingLink = dto.MeetingLink, Location = dto.Location,
        Feedback = dto.Feedback, Rating = dto.Rating
    };
}
