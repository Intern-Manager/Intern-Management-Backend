using InternManagement.Domain.Entities;

namespace InternManagement.Application.DTOs;

public record MentorshipDto(
    int MentorshipId,
    int MentorId,
    string MentorName,
    int InternId,
    string InternName,
    int ProgramId,
    string ProgramName,
    DateOnly StartDate,
    DateOnly? EndDate,
    string Status,
    DateTime CreatedAt);

public record MentorshipDetailDto(
    int MentorshipId,
    int MentorId,
    string MentorName,
    int InternId,
    string InternName,
    int ProgramId,
    string ProgramName,
    DateOnly StartDate,
    DateOnly? EndDate,
    string Status,
    DateTime CreatedAt);

public record CreateMentorshipRequest(
    int MentorId,
    int InternId,
    int ProgramId,
    DateOnly StartDate,
    DateOnly? EndDate);

public record UpdateMentorshipRequest(
    int? MentorId,
    int? InternId,
    int? ProgramId,
    DateOnly? StartDate,
    DateOnly? EndDate,
    string? Status);

public record MentorshipFilter(
    string? Status,
    int? MentorId,
    int? InternId,
    int? ProgramId);

public static class MentorshipDtoExtensions
{
    public static MentorshipDto ToDto(this Mentorship entity) => new(
        entity.MentorshipId, entity.MentorId, entity.Mentor?.FullName ?? "",
        entity.InternId, entity.Intern?.FullName ?? "",
        entity.ProgramId, entity.Program?.ProgramName ?? "",
        entity.StartDate, entity.EndDate, entity.Status, entity.CreatedAt);

    public static Mentorship ToEntity(this CreateMentorshipRequest dto) => new()
    {
        MentorId = dto.MentorId, InternId = dto.InternId, ProgramId = dto.ProgramId,
        StartDate = dto.StartDate, EndDate = dto.EndDate
    };
}
