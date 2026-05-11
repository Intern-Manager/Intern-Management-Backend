using InternManagement.Application.DTOs;
using InternManagement.Application.Repositories;
using InternManagement.Domain.Entities;
using InternManagement.Infrastructure.Persistence;
using InternManagement.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace InternManagement.Infrastructure.Auth;

public class EfRoleRepository : GenericRepository<Role>,
    InternManagement.Application.Auth.IRoleRepository,
    InternManagement.Application.Repositories.IRoleRepository
{
    public EfRoleRepository(AppDbContext db) : base(db) { }

    public Task<Role?> GetByNameAsync(string roleName, CancellationToken ct = default)
        => DbSet.AsNoTracking().FirstOrDefaultAsync(r => r.RoleName == roleName, ct);

    public async Task<IEnumerable<RoleDto>> GetAllDtoAsync(CancellationToken ct = default)
    {
        var roles = await DbSet.AsNoTracking().ToListAsync(ct);
        return roles.Select(r => new RoleDto(r.RoleId, r.RoleName, r.Description, r.CreatedAt));
    }
}
