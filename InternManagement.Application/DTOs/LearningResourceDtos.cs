using InternManagement.Domain.Entities;

namespace InternManagement.Application.DTOs;

public record LearningResourceDto(
    int ResourceId,
    int ProgramId,
    string Title,
    string? Description,
    string? ResourceUrl,
    string ResourceType,
    decimal? FileSizeMb,
    int UploadedBy,
    DateTime UploadedAt);

public record LearningResourceDetailDto(
    int ResourceId,
    int ProgramId,
    string ProgramName,
    string Title,
    string? Description,
    string? ResourceUrl,
    string ResourceType,
    decimal? FileSizeMb,
    int UploadedBy,
    string UploadedByName,
    DateTime UploadedAt);

public record CreateLearningResourceRequest(
    int ProgramId,
    string Title,
    string? Description,
    string? ResourceUrl,
    string ResourceType,
    decimal? FileSizeMb);

public record UpdateLearningResourceRequest(
    string? Title,
    string? Description,
    string? ResourceUrl,
    string? ResourceType,
    decimal? FileSizeMb);

public record LearningResourceFilter(
    string? Search,
    string? ResourceType,
    int? ProgramId);

public static class LearningResourceDtoExtensions
{
    public static LearningResourceDto ToDto(this LearningResource entity) => new(
        entity.ResourceId, entity.ProgramId, entity.Title, entity.Description,
        entity.ResourceUrl, entity.ResourceType, entity.FileSizeMb, entity.UploadedBy, entity.UploadedAt);

    public static LearningResource ToEntity(this CreateLearningResourceRequest dto, int uploadedBy) => new()
    {
        ProgramId = dto.ProgramId, Title = dto.Title, Description = dto.Description,
        ResourceUrl = dto.ResourceUrl, ResourceType = dto.ResourceType, FileSizeMb = dto.FileSizeMb,
        UploadedBy = uploadedBy
    };
}
