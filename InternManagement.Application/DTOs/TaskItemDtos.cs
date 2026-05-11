using InternManagement.Domain.Entities;

namespace InternManagement.Application.DTOs;

public record TaskItemDto(
    int TaskId,
    int InternId,
    int AssignedBy,
    int? ProgramId,
    string Title,
    string? Description,
    DateOnly? DueDate,
    string Priority,
    string Status,
    DateTime? CompletionDate,
    DateTime CreatedAt,
    DateTime? UpdatedAt);

public record TaskItemDetailDto(
    int TaskId,
    int InternId,
    string InternName,
    int AssignedBy,
    string AssignedByName,
    int? ProgramId,
    string? ProgramName,
    string Title,
    string? Description,
    DateOnly? DueDate,
    string Priority,
    string Status,
    DateTime? CompletionDate,
    DateTime CreatedAt,
    DateTime? UpdatedAt);

public record CreateTaskItemRequest(
    int InternId,
    int AssignedBy,
    int? ProgramId,
    string Title,
    string? Description,
    DateOnly? DueDate,
    string Priority,
    string Status);

public record UpdateTaskItemRequest(
    string? Title,
    string? Description,
    DateOnly? DueDate,
    string? Priority,
    string? Status,
    DateTime? CompletionDate);

public record TaskItemFilter(
    string? Search,
    string? Status,
    string? Priority,
    int? InternId,
    int? AssignedBy,
    int? ProgramId);

public static class TaskItemDtoExtensions
{
    public static TaskItemDto ToDto(this TaskItem entity) => new(
        entity.TaskId, entity.InternId, entity.AssignedBy, entity.ProgramId, entity.Title,
        entity.Description, entity.DueDate, entity.Priority, entity.Status, entity.CompletionDate,
        entity.CreatedAt, entity.UpdatedAt);

    public static TaskItem ToEntity(this CreateTaskItemRequest dto) => new()
    {
        InternId = dto.InternId, AssignedBy = dto.AssignedBy, ProgramId = dto.ProgramId,
        Title = dto.Title, Description = dto.Description, DueDate = dto.DueDate,
        Priority = dto.Priority, Status = dto.Status
    };
}
