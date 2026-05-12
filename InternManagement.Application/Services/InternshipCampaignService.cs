using InternManagement.Application.DTOs;
using InternManagement.Application.Repositories;

namespace InternManagement.Application.Services;

public class InternshipCampaignService : IInternshipCampaignService
{
    private readonly IInternshipCampaignRepository _repository;

    public InternshipCampaignService(IInternshipCampaignRepository repository) => _repository = repository;

    public async Task<PaginatedResult<InternshipCampaignDto>> GetAllAsync(PaginationRequest pagination, InternshipCampaignFilter? filter = null, CancellationToken ct = default)
    {
        var count = await _repository.CountAsync(filter, ct);
        var items = await _repository.GetAllDtoAsync(pagination, filter, ct);
        return items.ToPaginatedResult(pagination, count);
    }

    public async Task<InternshipCampaignDetailDto?> GetByIdAsync(int id, CancellationToken ct = default)
        => await _repository.GetDetailByIdAsync(id, ct);

    public async Task<InternshipCampaignDto?> CreateAsync(CreateInternshipCampaignRequest request, CancellationToken ct = default)
    {
        var entity = request.ToEntity(0);
        entity.CreatedAt = DateTime.UtcNow;
        await _repository.AddAsync(entity, ct);
        return entity.ToDto();
    }

    public async Task<InternshipCampaignDto?> UpdateAsync(int id, UpdateInternshipCampaignRequest request, CancellationToken ct = default)
    {
        var entity = await _repository.GetByIdAsync(id, ct);
        if (entity is null) return null;

        if (request.Title is not null) entity.Title = request.Title;
        if (request.Description is not null) entity.Description = request.Description;
        if (request.Requirements is not null) entity.Requirements = request.Requirements;
        if (request.NumberOfPositions.HasValue) entity.NumberOfPositions = request.NumberOfPositions.Value;
        if (request.Department is not null) entity.Department = request.Department;
        if (request.Location is not null) entity.Location = request.Location;
        if (request.StartDate.HasValue) entity.StartDate = request.StartDate;
        if (request.EndDate.HasValue) entity.EndDate = request.EndDate;
        if (request.ApplicationDeadline.HasValue) entity.ApplicationDeadline = request.ApplicationDeadline;
        if (request.Status is not null) entity.Status = request.Status;
        entity.UpdatedAt = DateTime.UtcNow;

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
