using InternManagement.Application.Auth;
using InternManagement.Application.DTOs;

namespace InternManagement.Application.Services;

public class UserService : IUserService
{
    private readonly Repositories.IUserRepository _repository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly Auth.IRoleRepository _roleRepository;
    private readonly IImageUploadService _imageUploadService;

    public UserService(
        Repositories.IUserRepository repository,
        IPasswordHasher passwordHasher,
        Auth.IRoleRepository roleRepository,
        IImageUploadService imageUploadService)
    {
        _repository = repository;
        _passwordHasher = passwordHasher;
        _roleRepository = roleRepository;
        _imageUploadService = imageUploadService;
    }

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
    {
        if (string.IsNullOrWhiteSpace(request.FullName) ||
            string.IsNullOrWhiteSpace(request.Email) ||
            string.IsNullOrWhiteSpace(request.Password))
            throw new InvalidOperationException("FullName, email, and password are required.");

        if (request.Password.Length < 6)
            throw new InvalidOperationException("Password must be at least 6 characters.");

        var existed = await _repository.FindByEmailAsync(request.Email, ct);
        if (existed is not null)
            throw new InvalidOperationException("Email already exists.");

        if (!await _roleRepository.ExistsAsync(request.RoleId, ct))
            throw new InvalidOperationException("Role does not exist.");

        var user = new Domain.Entities.User
        {
            FullName = request.FullName.Trim(),
            Email = request.Email.Trim(),
            PasswordHash = _passwordHasher.Hash(request.Password),
            Phone = request.Phone,
            RoleId = request.RoleId,
            Status = "Active",
            EmailVerified = false,
            CreatedAt = DateTime.UtcNow
        };

        await _repository.AddAsync(user, ct);
        return user.ToDto();
    }

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

    public async Task<bool> UpdateAvatarAsync(int id, string base64Image, CancellationToken ct = default)
    {
        var user = await _repository.GetByIdAsync(id, ct);
        if (user is null) return false;

        // Upload to Cloudinary and get the URL
        var avatarUrl = await _imageUploadService.UploadAvatarAsync(base64Image, ct);
        
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
