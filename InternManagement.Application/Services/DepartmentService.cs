using InternManagement.Application.DTOs;
using InternManagement.Application.Repositories;
using InternManagement.Domain.Entities;

namespace InternManagement.Application.Services;

public class DepartmentService : IDepartmentService
{
    private readonly IDepartmentRepository _repository;

    public DepartmentService(IDepartmentRepository repository) => _repository = repository;

    public async Task<PaginatedResult<DepartmentDto>> GetAllAsync(PaginationRequest pagination, DepartmentFilter? filter = null, CancellationToken ct = default)
    {
        var items = await _repository.GetAllDtoAsync(pagination, filter, ct);
        var totalCount = await _repository.CountAsync(filter, ct);
        var totalPages = (int)Math.Ceiling(totalCount / (double)pagination.PageSize);

        return new PaginatedResult<DepartmentDto>(
            items.ToList(), totalCount, pagination.Page, pagination.PageSize, totalPages);
    }

    public async Task<DepartmentDetailDto?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await _repository.GetDetailByIdAsync(id, ct);
    }

    public async Task<DepartmentDto?> CreateAsync(CreateDepartmentRequest request, CancellationToken ct = default)
    {
        var entity = request.ToEntity();
        entity.CreatedAt = DateTime.UtcNow;
        await _repository.AddAsync(entity, ct);
        return await _repository.GetAllDtoAsync(new PaginationRequest(1, 1), null, ct)
            .ContinueWith(t => t.Result.FirstOrDefault(d => d.DepartmentId == entity.DepartmentId), ct);
    }

    public async Task<DepartmentDto?> UpdateAsync(int id, UpdateDepartmentRequest request, CancellationToken ct = default)
    {
        var entity = await _repository.GetByIdAsync(id, ct);
        if (entity is null) return null;

        if (request.DepartmentName is not null) entity.DepartmentName = request.DepartmentName;
        if (request.Description is not null) entity.Description = request.Description;
        if (request.HeadUserId.HasValue) entity.HeadUserId = request.HeadUserId;
        if (request.Status is not null) entity.Status = request.Status;
        entity.UpdatedAt = DateTime.UtcNow;

        await _repository.UpdateAsync(entity, ct);
        return await _repository.GetAllDtoAsync(new PaginationRequest(1, 1), null, ct)
            .ContinueWith(t => t.Result.FirstOrDefault(d => d.DepartmentId == entity.DepartmentId), ct);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        if (!await _repository.ExistsAsync(id, ct)) return false;
        await _repository.DeleteAsync(id, ct);
        return true;
    }
}
