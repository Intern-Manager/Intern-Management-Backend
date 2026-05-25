using InternManagement.Application.Auth;
using InternManagement.Application.DTOs;

namespace InternManagement.Application.Services;

public class UserService : IUserService
{
    private readonly Repositories.IUserRepository _repository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly Auth.IRoleRepository _roleRepository;
    private readonly IImageUploadService _imageUploadService;
    private readonly IAuditLogService _auditLog;

    public UserService(
        Repositories.IUserRepository repository,
        IPasswordHasher passwordHasher,
        Auth.IRoleRepository roleRepository,
        IImageUploadService imageUploadService,
        IAuditLogService auditLog)
    {
        _repository = repository;
        _passwordHasher = passwordHasher;
        _roleRepository = roleRepository;
        _imageUploadService = imageUploadService;
        _auditLog = auditLog;
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

    public async Task<UserDetailDto?> GetByEmailAsync(string email, CancellationToken ct = default)
    {
        var user = await _repository.FindByEmailAsync(email, ct);
        if (user is null) return null;
        return await _repository.GetDetailByIdAsync(user.UserId, ct);
    }

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

        // Audit log
        await _auditLog.LogAsync(new AuditLogEntry
        {
            UserId = user.UserId,
            UserName = user.FullName,
            Action = "Created",
            EntityType = "User",
            EntityId = user.UserId,
            Description = $"Created user: {user.Email} (RoleId: {user.RoleId})",
            LogType = "Data"
        }, ct);

        return user.ToDto();
    }

    public async Task<UserDto?> UpdateAsync(int id, UpdateUserRequest request, CancellationToken ct = default)
    {
        var user = await _repository.GetByIdAsync(id, ct);
        if (user is null) return null;

        var changes = new List<string>();
        if (request.FullName is not null) { changes.Add($"FullName: {user.FullName} -> {request.FullName}"); user.FullName = request.FullName; }
        if (request.Phone is not null) { changes.Add($"Phone: {user.Phone} -> {request.Phone}"); user.Phone = request.Phone; }
        if (request.AvatarUrl is not null) { changes.Add("AvatarUrl updated"); user.AvatarUrl = request.AvatarUrl; }
        if (request.RoleId.HasValue) { changes.Add($"RoleId: {user.RoleId} -> {request.RoleId.Value}"); user.RoleId = request.RoleId.Value; }
        if (request.Status is not null) { changes.Add($"Status: {user.Status} -> {request.Status}"); user.Status = request.Status; }
        user.UpdatedAt = DateTime.UtcNow;

        await _repository.UpdateAsync(user, ct);

        // Audit log
        await _auditLog.LogAsync(new AuditLogEntry
        {
            Action = "Updated",
            EntityType = "User",
            EntityId = user.UserId,
            Description = $"Updated user {user.Email}: {string.Join(", ", changes)}",
            LogType = "Data"
        }, ct);

        return user.ToDto();
    }

    public async Task<bool> UpdateAvatarAsync(int id, string base64Image, CancellationToken ct = default)
    {
        var user = await _repository.GetByIdAsync(id, ct);
        if (user is null) return false;

        var avatarUrl = await _imageUploadService.UploadAvatarAsync(base64Image, ct);
        
        user.AvatarUrl = avatarUrl;
        user.UpdatedAt = DateTime.UtcNow;

        await _repository.UpdateAsync(user, ct);

        // Audit log
        await _auditLog.LogAsync(new AuditLogEntry
        {
            UserId = user.UserId,
            UserName = user.FullName,
            Action = "Updated Avatar",
            EntityType = "User",
            EntityId = user.UserId,
            Description = $"Updated avatar for user: {user.Email}",
            LogType = "Data"
        }, ct);

        return true;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        var user = await _repository.GetByIdAsync(id, ct);
        if (!await _repository.ExistsAsync(id, ct)) return false;
        await _repository.DeleteAsync(id, ct);

        // Audit log
        if (user != null)
        {
            await _auditLog.LogAsync(new AuditLogEntry
            {
                UserId = user.UserId,
                UserName = user.FullName,
                Action = "Deleted",
                EntityType = "User",
                EntityId = user.UserId,
                Description = $"Deleted user: {user.Email}",
                LogType = "Data"
            }, ct);
        }

        return true;
    }

    public async Task<IEnumerable<ChatContactDto>> GetChatContactsAsync(int currentUserId, CancellationToken ct = default)
    {
        return await _repository.GetChatContactsAsync(currentUserId, ct);
    }
}
