using InternManagement.Application.DTOs;
using InternManagement.Application.Repositories;
using InternManagement.Domain.Entities;
using InternManagement.Infrastructure.Persistence;
using InternManagement.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace InternManagement.Infrastructure.Auth;

public class EfUserRepository : GenericRepository<User>, 
    InternManagement.Application.Auth.IUserRepository,
    InternManagement.Application.Repositories.IUserRepository
{
    public EfUserRepository(AppDbContext db) : base(db) { }

    public Task<User?> FindByEmailAsync(string email, CancellationToken ct)
        => DbSet.AsNoTracking().FirstOrDefaultAsync(u => u.Email == email, ct);

    public Task<User?> FindByIdAsync(int id, CancellationToken ct)
        => DbSet.AsNoTracking().FirstOrDefaultAsync(u => u.UserId == id, ct);

    public async Task<UserDetailDto?> GetDetailByIdAsync(int id, CancellationToken ct = default)
    {
        var user = await DbSet.AsNoTracking().FirstOrDefaultAsync(u => u.UserId == id, ct);
        if (user is null) return null;

        var roleName = await _context.Roles
            .AsNoTracking()
            .Where(r => r.RoleId == user.RoleId)
            .Select(r => r.RoleName)
            .FirstOrDefaultAsync(ct);

        return user.ToDetailDto(roleName);
    }

    public async Task<IEnumerable<UserDto>> GetAllDtoAsync(PaginationRequest pagination, UserFilter? filter = null, CancellationToken ct = default)
    {
        var query = DbSet.AsQueryable();

        if (!string.IsNullOrWhiteSpace(filter?.Search))
            query = query.Where(u => u.FullName.Contains(filter.Search) || u.Email.Contains(filter.Search));
        if (!string.IsNullOrWhiteSpace(filter?.Status))
            query = query.Where(u => u.Status == filter.Status);
        if (filter?.RoleId.HasValue == true)
            query = query.Where(u => u.RoleId == filter.RoleId.Value);

        return await query
            .OrderByDescending(u => u.CreatedAt)
            .Skip((pagination.Page - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .Select(u => new UserDto(
                u.UserId, u.FullName, u.Email, u.Phone, u.AvatarUrl,
                u.RoleId, u.Status, u.EmailVerified, u.EmailVerifiedAt,
                u.LastLogin, u.CreatedAt, u.UpdatedAt))
            .ToListAsync(ct);
    }

    public async Task<int> CountAsync(UserFilter? filter = null, CancellationToken ct = default)
    {
        var query = DbSet.AsQueryable();

        if (!string.IsNullOrWhiteSpace(filter?.Search))
            query = query.Where(u => u.FullName.Contains(filter.Search) || u.Email.Contains(filter.Search));
        if (!string.IsNullOrWhiteSpace(filter?.Status))
            query = query.Where(u => u.Status == filter.Status);
        if (filter?.RoleId.HasValue == true)
            query = query.Where(u => u.RoleId == filter.RoleId.Value);

        return await query.CountAsync(ct);
    }

    public async Task<IEnumerable<ChatContactDto>> GetChatContactsAsync(int currentUserId, CancellationToken ct = default)
    {
        var users = await DbSet
            .AsNoTracking()
            .Where(u => u.UserId != currentUserId && u.Status == "Active")
            .OrderBy(u => u.FullName)
            .Select(u => new {
                u.UserId,
                u.FullName,
                u.Email,
                u.AvatarUrl,
                u.RoleId,
                LastMessage = (string?)null,
                LastMessageTime = (DateTime?)null,
                UnreadCount = 0
            })
            .ToListAsync(ct);

        var roles = await _context.Roles
            .AsNoTracking()
            .ToDictionaryAsync(r => r.RoleId);

        return users.Select(u => new ChatContactDto(
            u.UserId, u.FullName, u.Email, u.AvatarUrl, u.RoleId,
            roles.TryGetValue(u.RoleId, out var r) ? r.RoleName : null,
            u.LastMessage, u.LastMessageTime, u.UnreadCount));
    }
}
