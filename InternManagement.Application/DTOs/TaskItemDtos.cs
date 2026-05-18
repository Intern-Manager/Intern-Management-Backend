using InternManagement.Domain.Entities;

namespace InternManagement.Application.DTOs;

public record TaskItemDto(
    int TaskId,
    int InternId,
    string? InternName,
    int AssignedBy,
    string? AssignedByName,
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

// Task Submission DTOs
public record TaskSubmissionDto(
    int SubmissionId,
    int TaskId,
    string? TaskTitle,
    int InternId,
    string? InternName,
    string? SubmissionUrl,
    string? SubmissionText,
    string? Comments,
    string Status,
    int? GradedBy,
    string? GradedByName,
    int? Score,
    string? Feedback,
    DateTime SubmittedAt,
    DateTime? GradedAt);

public record CreateTaskSubmissionRequest(
    int TaskId,
    int InternId,
    string? SubmissionUrl,
    string? SubmissionText,
    string? Comments);

public record GradeSubmissionRequest(
    int Score,
    string? Feedback);

public static class TaskItemDtoExtensions
{
    public static TaskItemDto ToDto(this TaskItem entity, string? internName = null, string? assignedByName = null, string? programName = null) => new(
        entity.TaskId, entity.InternId, internName, entity.AssignedBy, assignedByName,
        entity.ProgramId, programName, entity.Title, entity.Description,
        entity.DueDate, entity.Priority, entity.Status, entity.CompletionDate,
        entity.CreatedAt, entity.UpdatedAt);

    public static TaskItem ToEntity(this CreateTaskItemRequest dto) => new()
    {
        InternId = dto.InternId, AssignedBy = dto.AssignedBy, ProgramId = dto.ProgramId,
        Title = dto.Title, Description = dto.Description, DueDate = dto.DueDate,
        Priority = dto.Priority, Status = dto.Status
    };

    public static TaskSubmissionDto ToDto(this TaskSubmission entity, string? taskTitle = null, string? internName = null, string? gradedByName = null) => new(
        entity.SubmissionId, entity.TaskId, taskTitle, entity.InternId, internName,
        entity.SubmissionUrl, entity.SubmissionText, entity.Comments, entity.Status,
        entity.GradedBy, gradedByName, entity.Score, entity.Feedback,
        entity.SubmittedAt, entity.GradedAt);

    public static TaskSubmission ToEntity(this CreateTaskSubmissionRequest dto) => new()
    {
        TaskId = dto.TaskId, InternId = dto.InternId,
        SubmissionUrl = dto.SubmissionUrl, SubmissionText = dto.SubmissionText,
        Comments = dto.Comments, Status = "Submitted", SubmittedAt = DateTime.UtcNow
    };
}
