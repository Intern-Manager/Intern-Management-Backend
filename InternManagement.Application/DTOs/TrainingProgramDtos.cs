using InternManagement.Domain.Entities;

namespace InternManagement.Application.DTOs;

public record TrainingProgramDto(
    int ProgramId,
    string ProgramName,
    string? Description,
    string? Objectives,
    int? DurationWeeks,
    DateOnly? StartDate,
    DateOnly? EndDate,
    int CoordinatorId,
    string Status,
    DateTime CreatedAt,
    DateTime? UpdatedAt);

public record TrainingProgramDetailDto(
    int ProgramId,
    string ProgramName,
    string? Description,
    string? Objectives,
    int? DurationWeeks,
    DateOnly? StartDate,
    DateOnly? EndDate,
    int CoordinatorId,
    string CoordinatorName,
    string Status,
    DateTime CreatedAt,
    DateTime? UpdatedAt);

public record CreateTrainingProgramRequest(
    string ProgramName,
    string? Description,
    string? Objectives,
    int? DurationWeeks,
    DateOnly? StartDate,
    DateOnly? EndDate,
    int CoordinatorId,
    string Status);

public record UpdateTrainingProgramRequest(
    string? ProgramName,
    string? Description,
    string? Objectives,
    int? DurationWeeks,
    DateOnly? StartDate,
    DateOnly? EndDate,
    string? Status);

public record TrainingProgramFilter(
    string? Search,
    string? Status,
    int? CoordinatorId);

public static class TrainingProgramDtoExtensions
{
    public static TrainingProgramDto ToDto(this TrainingProgram entity) => new(
        entity.ProgramId, entity.ProgramName, entity.Description, entity.Objectives,
        entity.DurationWeeks, entity.StartDate, entity.EndDate, entity.CoordinatorId,
        entity.Status, entity.CreatedAt, entity.UpdatedAt);

    public static TrainingProgram ToEntity(this CreateTrainingProgramRequest dto) => new()
    {
        ProgramName = dto.ProgramName, Description = dto.Description, Objectives = dto.Objectives,
        DurationWeeks = dto.DurationWeeks, StartDate = dto.StartDate, EndDate = dto.EndDate,
        CoordinatorId = dto.CoordinatorId, Status = dto.Status
    };
}
