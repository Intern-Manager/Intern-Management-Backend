using InternManagement.Application.DTOs;
using InternManagement.Application.Repositories;

namespace InternManagement.Application.Services;

public class CampaignApplicationService : ICampaignApplicationService
{
    private readonly ICampaignApplicationRepository _repository;

    public CampaignApplicationService(ICampaignApplicationRepository repository) => _repository = repository;

    public async Task<PaginatedResult<CampaignApplicationDto>> GetAllAsync(PaginationRequest pagination, CampaignApplicationFilter? filter = null, CancellationToken ct = default)
    {
        var count = await _repository.CountAsync(filter, ct);
        var items = await _repository.GetAllDtoAsync(pagination, filter, ct);
        return items.ToPaginatedResult(pagination, count);
    }

    public async Task<CampaignApplicationDetailDto?> GetByIdAsync(int id, CancellationToken ct = default)
        => await _repository.GetDetailByIdAsync(id, ct);

    public async Task<CampaignApplicationDto?> CreateAsync(CreateCampaignApplicationRequest request, CancellationToken ct = default)
    {
        var entity = request.ToEntity();
        entity.AppliedDate = DateTime.UtcNow;
        await _repository.AddAsync(entity, ct);
        return entity.ToDto();
    }

    public async Task<CampaignApplicationDto?> UpdateAsync(int id, UpdateCampaignApplicationRequest request, CancellationToken ct = default)
    {
        var entity = await _repository.GetByIdAsync(id, ct);
        if (entity is null) return null;

        if (request.Status is not null) entity.Status = request.Status;
        if (request.ReviewedBy.HasValue) entity.ReviewedBy = request.ReviewedBy;
        if (request.Notes is not null) entity.Notes = request.Notes;
        if (entity.Status != "Pending") entity.ReviewedDate = DateTime.UtcNow;

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
