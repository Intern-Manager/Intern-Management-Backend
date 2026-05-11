using InternManagement.Domain.Entities;

namespace InternManagement.Application.DTOs;

public record RoleDto(
    int RoleId,
    string RoleName,
    string? Description,
    DateTime CreatedAt);

public record CreateRoleRequest(string RoleName, string? Description);
public record UpdateRoleRequest(string RoleName, string? Description);

public static class RoleDtoExtensions
{
    public static RoleDto ToDto(this Role entity) => new(entity.RoleId, entity.RoleName, entity.Description, entity.CreatedAt);
    public static Role ToEntity(this CreateRoleRequest dto) => new() { RoleName = dto.RoleName, Description = dto.Description };
}
