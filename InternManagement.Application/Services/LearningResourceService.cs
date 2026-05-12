using InternManagement.Application.DTOs;
using InternManagement.Application.Repositories;

namespace InternManagement.Application.Services;

public class LearningResourceService : ILearningResourceService
{
    private readonly ILearningResourceRepository _repository;

    public LearningResourceService(ILearningResourceRepository repository) => _repository = repository;

    public async Task<PaginatedResult<LearningResourceDto>> GetAllAsync(PaginationRequest pagination, LearningResourceFilter? filter = null, CancellationToken ct = default)
    {
        var count = await _repository.CountAsync(filter, ct);
        var items = await _repository.GetAllDtoAsync(pagination, filter, ct);
        return items.ToPaginatedResult(pagination, count);
    }

    public async Task<LearningResourceDetailDto?> GetByIdAsync(int id, CancellationToken ct = default)
        => await _repository.GetDetailByIdAsync(id, ct);

    public async Task<LearningResourceDto?> CreateAsync(CreateLearningResourceRequest request, CancellationToken ct = default)
        => throw new NotImplementedException("CreateAsync requires uploadedBy parameter");

    public async Task<LearningResourceDto?> UpdateAsync(int id, UpdateLearningResourceRequest request, CancellationToken ct = default)
    {
        var entity = await _repository.GetByIdAsync(id, ct);
        if (entity is null) return null;

        if (request.Title is not null) entity.Title = request.Title;
        if (request.Description is not null) entity.Description = request.Description;
        if (request.ResourceUrl is not null) entity.ResourceUrl = request.ResourceUrl;
        if (request.ResourceType is not null) entity.ResourceType = request.ResourceType;
        if (request.FileSizeMb.HasValue) entity.FileSizeMb = request.FileSizeMb;

        await _repository.UpdateAsync(entity, ct);
        return entity.ToDto();
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        if (!await _repository.ExistsAsync(id, ct)) return false;
        await _repository.DeleteAsync(id, ct);
        return true;
    }
}
