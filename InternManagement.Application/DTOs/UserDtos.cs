using InternManagement.Domain.Entities;

namespace InternManagement.Application.DTOs;

public record UserDto(
    int UserId,
    string FullName,
    string Email,
    string? Phone,
    string? AvatarUrl,
    int RoleId,
    string Status,
    bool EmailVerified,
    DateTime? EmailVerifiedAt,
    DateTime? LastLogin,
    DateTime CreatedAt,
    DateTime? UpdatedAt);

public record UserDetailDto(
    int UserId,
    string FullName,
    string Email,
    string? Phone,
    string? AvatarUrl,
    int RoleId,
    string? RoleName,
    string Status,
    bool EmailVerified,
    DateTime? EmailVerifiedAt,
    DateTime? LastLogin,
    DateTime CreatedAt,
    DateTime? UpdatedAt);

public record CreateUserRequest(
    string FullName,
    string Email,
    string Password,
    int RoleId,
    string? Phone = null);

public record UpdateUserRequest(
    string? FullName,
    string? Phone,
    string? AvatarUrl,
    int? RoleId,
    string? Status);

public record UserFilter(string? Search, string? Status, int? RoleId);

public static class UserDtoExtensions
{
    public static UserDto ToDto(this User entity) => new(
        entity.UserId, entity.FullName, entity.Email, entity.Phone, entity.AvatarUrl,
        entity.RoleId, entity.Status, entity.EmailVerified, entity.EmailVerifiedAt,
        entity.LastLogin, entity.CreatedAt, entity.UpdatedAt);

    public static UserDetailDto ToDetailDto(this User entity, string? roleName = null) => new(
        entity.UserId, entity.FullName, entity.Email, entity.Phone, entity.AvatarUrl,
        entity.RoleId, roleName, entity.Status, entity.EmailVerified, entity.EmailVerifiedAt,
        entity.LastLogin, entity.CreatedAt, entity.UpdatedAt);

    public static User ToEntity(this CreateUserRequest dto, string passwordHash) => new()
    {
        FullName = dto.FullName, Email = dto.Email, PasswordHash = passwordHash,
        RoleId = dto.RoleId, Phone = dto.Phone
    };
}
