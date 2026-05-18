using InternManagement.Domain.Entities;

namespace InternManagement.Application.DTOs;

public record DepartmentDto(
    int DepartmentId,
    string DepartmentName,
    string? Description,
    int? HeadUserId,
    string? HeadUserName,
    int MemberCount,
    int InternCount,
    string Status,
    DateTime CreatedAt,
    DateTime? UpdatedAt);

public record DepartmentDetailDto(
    int DepartmentId,
    string DepartmentName,
    string? Description,
    int? HeadUserId,
    string? HeadUserName,
    string Status,
    DateTime CreatedAt,
    DateTime? UpdatedAt);

public record CreateDepartmentRequest(
    string DepartmentName,
    string? Description,
    int? HeadUserId);

public record UpdateDepartmentRequest(
    string? DepartmentName,
    string? Description,
    int? HeadUserId,
    string? Status);

public record DepartmentFilter(
    string? Search,
    string? Status);

public static class DepartmentDtoExtensions
{
    public static DepartmentDto ToDto(this Department entity, string? headUserName = null, int memberCount = 0, int internCount = 0) =>
        new(entity.DepartmentId, entity.DepartmentName, entity.Description,
            entity.HeadUserId, headUserName, memberCount, internCount,
            entity.Status, entity.CreatedAt, entity.UpdatedAt);

    public static DepartmentDetailDto ToDetailDto(this Department entity, string? headUserName = null) =>
        new(entity.DepartmentId, entity.DepartmentName, entity.Description,
            entity.HeadUserId, headUserName, entity.Status, entity.CreatedAt, entity.UpdatedAt);

    public static Department ToEntity(this CreateDepartmentRequest dto) => new()
    {
        DepartmentName = dto.DepartmentName,
        Description = dto.Description,
        HeadUserId = dto.HeadUserId
    };
}
