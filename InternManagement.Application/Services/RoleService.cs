using InternManagement.Application.DTOs;
using InternManagement.Application.Repositories;

namespace InternManagement.Application.Services;

public class RoleService : IRoleService
{
    private readonly IRoleRepository _repository;

    public RoleService(IRoleRepository repository) => _repository = repository;

    public async Task<IEnumerable<RoleDto>> GetAllAsync(CancellationToken ct = default)
    {
        var roles = await _repository.GetAllAsync(ct);
        return roles.Select(r => r.ToDto());
    }

    public async Task<RoleDto?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var role = await _repository.GetByIdAsync(id, ct);
        return role?.ToDto();
    }

    public async Task<RoleDto> CreateAsync(CreateRoleRequest request, CancellationToken ct = default)
    {
        var role = request.ToEntity();
        await _repository.AddAsync(role, ct);
        return role.ToDto();
    }

    public async Task<RoleDto?> UpdateAsync(int id, UpdateRoleRequest request, CancellationToken ct = default)
    {
        var role = await _repository.GetByIdAsync(id, ct);
        if (role is null) return null;

        role.RoleName = request.RoleName ?? role.RoleName;
        role.Description = request.Description;
        await _repository.UpdateAsync(role, ct);
        return role.ToDto();
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        if (!await _repository.ExistsAsync(id, ct)) return false;
        await _repository.DeleteAsync(id, ct);
        return true;
    }
}
