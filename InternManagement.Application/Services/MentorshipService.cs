using InternManagement.Application.DTOs;
using InternManagement.Application.Repositories;

namespace InternManagement.Application.Services;

public class MentorshipService : IMentorshipService
{
    private readonly IMentorshipRepository _repository;

    public MentorshipService(IMentorshipRepository repository) => _repository = repository;

    public async Task<PaginatedResult<MentorshipDto>> GetAllAsync(PaginationRequest pagination, MentorshipFilter? filter = null, CancellationToken ct = default)
    {
        var count = await _repository.CountAsync(filter, ct);
        var items = await _repository.GetAllDtoAsync(pagination, filter, ct);
        return items.ToPaginatedResult(pagination, count);
    }

    public async Task<MentorshipDetailDto?> GetByIdAsync(int id, CancellationToken ct = default)
        => await _repository.GetDetailByIdAsync(id, ct);

    public async Task<MentorshipDto?> CreateAsync(CreateMentorshipRequest request, CancellationToken ct = default)
    {
        var entity = request.ToEntity();
        entity.CreatedAt = DateTime.UtcNow;
        await _repository.AddAsync(entity, ct);
        return entity.ToDto();
    }

    public async Task<MentorshipDto?> UpdateAsync(int id, UpdateMentorshipRequest request, CancellationToken ct = default)
    {
        var entity = await _repository.GetByIdAsync(id, ct);
        if (entity is null) return null;

        if (request.MentorId.HasValue) entity.MentorId = request.MentorId.Value;
        if (request.InternId.HasValue) entity.InternId = request.InternId.Value;
        if (request.ProgramId.HasValue) entity.ProgramId = request.ProgramId.Value;
        if (request.StartDate.HasValue) entity.StartDate = request.StartDate.Value;
        if (request.EndDate.HasValue) entity.EndDate = request.EndDate;
        if (request.Status is not null) entity.Status = request.Status;

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
