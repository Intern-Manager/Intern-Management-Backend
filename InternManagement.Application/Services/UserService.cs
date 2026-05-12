using InternManagement.Application.DTOs;
using InternManagement.Application.Repositories;

namespace InternManagement.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _repository;

    public UserService(IUserRepository repository) => _repository = repository;

    public async Task<PaginatedResult<UserDto>> GetAllAsync(PaginationRequest pagination, UserFilter? filter = null, CancellationToken ct = default)
    {
        var count = await _repository.CountAsync(filter, ct);
        var users = await _repository.GetAllDtoAsync(pagination, filter, ct);
        return users.Select(u => new UserDto(
            u.UserId, u.FullName, u.Email, u.Phone, u.AvatarUrl,
            u.RoleId, u.Status, u.EmailVerified, u.EmailVerifiedAt,
            u.LastLogin, u.CreatedAt, u.UpdatedAt))
            .ToPaginatedResult(pagination, count);
    }

    public async Task<UserDetailDto?> GetByIdAsync(int id, CancellationToken ct = default)
        => await _repository.GetDetailByIdAsync(id, ct);

    public async Task<UserDto?> CreateAsync(CreateUserRequest request, CancellationToken ct = default)
        => throw new NotImplementedException("Use AuthService for user registration");

    public async Task<UserDto?> UpdateAsync(int id, UpdateUserRequest request, CancellationToken ct = default)
    {
        var user = await _repository.GetByIdAsync(id, ct);
        if (user is null) return null;

        if (request.FullName is not null) user.FullName = request.FullName;
        if (request.Phone is not null) user.Phone = request.Phone;
        if (request.AvatarUrl is not null) user.AvatarUrl = request.AvatarUrl;
        if (request.RoleId.HasValue) user.RoleId = request.RoleId.Value;
        if (request.Status is not null) user.Status = request.Status;
        user.UpdatedAt = DateTime.UtcNow;

        await _repository.UpdateAsync(user, ct);
        return user.ToDto();
    }

    public async Task<bool> UpdateAvatarAsync(int id, string avatarUrl, CancellationToken ct = default)
    {
        var user = await _repository.GetByIdAsync(id, ct);
        if (user is null) return false;

        user.AvatarUrl = avatarUrl;
        user.UpdatedAt = DateTime.UtcNow;

        await _repository.UpdateAsync(user, ct);
        return true;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        if (!await _repository.ExistsAsync(id, ct)) return false;
        await _repository.DeleteAsync(id, ct);
        return true;
    }
}
