using System.ComponentModel.DataAnnotations;
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
    [Required(ErrorMessage = "Full name is required")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Full name must be between 2 and 100 characters")]
    [RegularExpression(@"^[a-zA-ZÀ-ỹ\s]+$", ErrorMessage = "Full name can only contain letters and spaces")]
    string FullName,

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    [StringLength(100)]
    string Email,

    [Required(ErrorMessage = "Password is required")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters")]
    string Password,

    [Range(1, int.MaxValue, ErrorMessage = "Role is required")]
    int RoleId,

    [Phone(ErrorMessage = "Invalid phone format")]
    [StringLength(20, MinimumLength = 10, ErrorMessage = "Phone must be between 10 and 20 characters")]
    string? Phone = null);

public record UpdateUserRequest(
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Full name must be between 2 and 100 characters")]
    [RegularExpression(@"^[a-zA-ZÀ-ỹ\s]+$", ErrorMessage = "Full name can only contain letters and spaces")]
    string? FullName,

    [Phone(ErrorMessage = "Invalid phone format")]
    [StringLength(20, MinimumLength = 10, ErrorMessage = "Phone must be between 10 and 20 characters")]
    string? Phone,

    string? AvatarUrl,

    [Range(1, int.MaxValue, ErrorMessage = "Invalid role")]
    int? RoleId,

    [RegularExpression(@"^(Active|Inactive)$", ErrorMessage = "Status must be 'Active' or 'Inactive'")]
    string? Status);

public record UpdateAvatarRequest(
    [Required]
    string AvatarUrl);

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
