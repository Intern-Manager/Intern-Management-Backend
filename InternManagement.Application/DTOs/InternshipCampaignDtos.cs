using InternManagement.Domain.Entities;

namespace InternManagement.Application.DTOs;

public record InternshipCampaignDto(
    int CampaignId,
    string Title,
    string? Description,
    string? Requirements,
    int NumberOfPositions,
    string? Department,
    string? Location,
    DateOnly? StartDate,
    DateOnly? EndDate,
    DateOnly? ApplicationDeadline,
    string Status,
    int CreatedBy,
    DateTime CreatedAt,
    DateTime? UpdatedAt);

public record InternshipCampaignDetailDto(
    int CampaignId,
    string Title,
    string? Description,
    string? Requirements,
    int NumberOfPositions,
    string? Department,
    string? Location,
    DateOnly? StartDate,
    DateOnly? EndDate,
    DateOnly? ApplicationDeadline,
    string Status,
    int CreatedBy,
    string? CreatedByName,
    DateTime CreatedAt,
    DateTime? UpdatedAt);

public record CreateInternshipCampaignRequest(
    string Title,
    string? Description,
    string? Requirements,
    int NumberOfPositions,
    string? Department,
    string? Location,
    DateOnly? StartDate,
    DateOnly? EndDate,
    DateOnly? ApplicationDeadline,
    string Status);

public record UpdateInternshipCampaignRequest(
    string? Title,
    string? Description,
    string? Requirements,
    int? NumberOfPositions,
    string? Department,
    string? Location,
    DateOnly? StartDate,
    DateOnly? EndDate,
    DateOnly? ApplicationDeadline,
    string? Status);

public record InternshipCampaignFilter(
    string? Search,
    string? Status,
    string? Department,
    string? Location);

public static class InternshipCampaignDtoExtensions
{
    public static InternshipCampaignDto ToDto(this InternshipCampaign entity) => new(
        entity.CampaignId, entity.Title, entity.Description, entity.Requirements, entity.NumberOfPositions,
        entity.Department, entity.Location, entity.StartDate, entity.EndDate, entity.ApplicationDeadline,
        entity.Status, entity.CreatedBy, entity.CreatedAt, entity.UpdatedAt);

    public static InternshipCampaign ToEntity(this CreateInternshipCampaignRequest dto, int createdBy) => new()
    {
        Title = dto.Title, Description = dto.Description, Requirements = dto.Requirements,
        NumberOfPositions = dto.NumberOfPositions, Department = dto.Department, Location = dto.Location,
        StartDate = dto.StartDate, EndDate = dto.EndDate, ApplicationDeadline = dto.ApplicationDeadline,
        Status = dto.Status, CreatedBy = createdBy
    };
}
