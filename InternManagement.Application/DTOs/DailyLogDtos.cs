using InternManagement.Domain.Entities;

namespace InternManagement.Application.DTOs;

public record DailyLogDto(
    int LogId,
    int InternId,
    int? MentorId,
    DateOnly LogDate,
    string ActivityDescription,
    decimal? HoursWorked,
    string? ChallengesFaced,
    string? MentorFeedback,
    decimal? KpiScore,
    DateTime CreatedAt,
    DateTime? UpdatedAt);

public record DailyLogDetailDto(
    int LogId,
    int InternId,
    string InternName,
    int? MentorId,
    string? MentorName,
    DateOnly LogDate,
    string ActivityDescription,
    decimal? HoursWorked,
    string? ChallengesFaced,
    string? MentorFeedback,
    decimal? KpiScore,
    DateTime CreatedAt,
    DateTime? UpdatedAt);

public record CreateDailyLogRequest(
    int InternId,
    int? MentorId,
    DateOnly LogDate,
    string ActivityDescription,
    decimal? HoursWorked,
    string? ChallengesFaced);

public record UpdateDailyLogRequest(
    DateOnly? LogDate,
    string? ActivityDescription,
    decimal? HoursWorked,
    string? ChallengesFaced,
    string? MentorFeedback,
    decimal? KpiScore);

public record DailyLogFilter(
    string? Search,
    int? InternId,
    int? MentorId,
    DateOnly? FromDate,
    DateOnly? ToDate);

public static class DailyLogDtoExtensions
{
    public static DailyLogDto ToDto(this DailyLog entity) => new(
        entity.LogId, entity.InternId, entity.MentorId, entity.LogDate, entity.ActivityDescription,
        entity.HoursWorked, entity.ChallengesFaced, entity.MentorFeedback, entity.KpiScore,
        entity.CreatedAt, entity.UpdatedAt);

    public static DailyLog ToEntity(this CreateDailyLogRequest dto) => new()
    {
        InternId = dto.InternId, MentorId = dto.MentorId, LogDate = dto.LogDate,
        ActivityDescription = dto.ActivityDescription, HoursWorked = dto.HoursWorked,
        ChallengesFaced = dto.ChallengesFaced
    };
}
