using InternManagement.Domain.Entities;

namespace InternManagement.Application.DTOs;

public record AssessmentDto(
    int AssessmentId,
    int InternId,
    int MentorId,
    int? ProgramId,
    DateOnly AssessmentDate,
    string AssessmentType,
    decimal? TechnicalSkillsScore,
    decimal? SoftSkillsScore,
    decimal? CommunicationScore,
    decimal? TeamworkScore,
    decimal? OverallRating,
    string? Strengths,
    string? AreasForImprovement,
    string? Comments,
    DateTime CreatedAt);

public record AssessmentDetailDto(
    int AssessmentId,
    int InternId,
    string InternName,
    int MentorId,
    string MentorName,
    int? ProgramId,
    string? ProgramName,
    DateOnly AssessmentDate,
    string AssessmentType,
    decimal? TechnicalSkillsScore,
    decimal? SoftSkillsScore,
    decimal? CommunicationScore,
    decimal? TeamworkScore,
    decimal? OverallRating,
    string? Strengths,
    string? AreasForImprovement,
    string? Comments,
    DateTime CreatedAt);

public record CreateAssessmentRequest(
    int InternId,
    int MentorId,
    int? ProgramId,
    DateOnly AssessmentDate,
    string AssessmentType,
    decimal? TechnicalSkillsScore,
    decimal? SoftSkillsScore,
    decimal? CommunicationScore,
    decimal? TeamworkScore,
    decimal? OverallRating,
    string? Strengths,
    string? AreasForImprovement,
    string? Comments);

public record UpdateAssessmentRequest(
    DateOnly? AssessmentDate,
    string? AssessmentType,
    decimal? TechnicalSkillsScore,
    decimal? SoftSkillsScore,
    decimal? CommunicationScore,
    decimal? TeamworkScore,
    decimal? OverallRating,
    string? Strengths,
    string? AreasForImprovement,
    string? Comments);

public record AssessmentFilter(
    string? AssessmentType,
    int? InternId,
    int? MentorId,
    int? ProgramId,
    DateOnly? FromDate,
    DateOnly? ToDate);

public static class AssessmentDtoExtensions
{
    public static AssessmentDto ToDto(this Assessment entity) => new(
        entity.AssessmentId, entity.InternId, entity.MentorId, entity.ProgramId,
        entity.AssessmentDate, entity.AssessmentType, entity.TechnicalSkillsScore,
        entity.SoftSkillsScore, entity.CommunicationScore, entity.TeamworkScore,
        entity.OverallRating, entity.Strengths, entity.AreasForImprovement, entity.Comments,
        entity.CreatedAt);

    public static Assessment ToEntity(this CreateAssessmentRequest dto) => new()
    {
        InternId = dto.InternId, MentorId = dto.MentorId, ProgramId = dto.ProgramId,
        AssessmentDate = dto.AssessmentDate, AssessmentType = dto.AssessmentType,
        TechnicalSkillsScore = dto.TechnicalSkillsScore, SoftSkillsScore = dto.SoftSkillsScore,
        CommunicationScore = dto.CommunicationScore, TeamworkScore = dto.TeamworkScore,
        OverallRating = dto.OverallRating, Strengths = dto.Strengths,
        AreasForImprovement = dto.AreasForImprovement, Comments = dto.Comments
    };
}
