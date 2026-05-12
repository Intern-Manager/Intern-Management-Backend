using InternManagement.Application.DTOs;
using InternManagement.Application.Repositories;

namespace InternManagement.Application.Services;

public class CommunicationService : ICommunicationService
{
    private readonly ICommunicationRepository _repository;

    public CommunicationService(ICommunicationRepository repository) => _repository = repository;

    public async Task<PaginatedResult<CommunicationDto>> GetAllAsync(PaginationRequest pagination, CommunicationFilter? filter = null, CancellationToken ct = default)
    {
        var count = await _repository.CountAsync(filter, ct);
        var items = await _repository.GetAllDtoAsync(pagination, filter, ct);
        return items.ToPaginatedResult(pagination, count);
    }

    public async Task<CommunicationDetailDto?> GetByIdAsync(int id, CancellationToken ct = default)
        => await _repository.GetDetailByIdAsync(id, ct);

    public async Task<CommunicationDto?> CreateAsync(CreateCommunicationRequest request, CancellationToken ct = default)
    {
        var entity = request.ToEntity();
        entity.SentAt = DateTime.UtcNow;
        await _repository.AddAsync(entity, ct);
        return entity.ToDto();
    }

    public async Task<CommunicationDto?> UpdateAsync(int id, UpdateCommunicationRequest request, CancellationToken ct = default)
    {
        var entity = await _repository.GetByIdAsync(id, ct);
        if (entity is null) return null;

        if (request.IsRead.HasValue) entity.IsRead = request.IsRead.Value;
        if (request.ReadAt.HasValue) entity.ReadAt = request.ReadAt;

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
