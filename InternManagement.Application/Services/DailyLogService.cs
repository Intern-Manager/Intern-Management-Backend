using InternManagement.Application.DTOs;
using InternManagement.Application.Repositories;

namespace InternManagement.Application.Services;

public class DailyLogService : IDailyLogService
{
    private readonly IDailyLogRepository _repository;

    public DailyLogService(IDailyLogRepository repository) => _repository = repository;

    public async Task<PaginatedResult<DailyLogDto>> GetAllAsync(PaginationRequest pagination, DailyLogFilter? filter = null, CancellationToken ct = default)
    {
        var count = await _repository.CountAsync(filter, ct);
        var items = await _repository.GetAllDtoAsync(pagination, filter, ct);
        return items.ToPaginatedResult(pagination, count);
    }

    public async Task<DailyLogDetailDto?> GetByIdAsync(int id, CancellationToken ct = default)
        => await _repository.GetDetailByIdAsync(id, ct);

    public async Task<DailyLogDto?> CreateAsync(CreateDailyLogRequest request, CancellationToken ct = default)
    {
        var entity = request.ToEntity();
        entity.CreatedAt = DateTime.UtcNow;
        await _repository.AddAsync(entity, ct);
        return entity.ToDto();
    }

    public async Task<DailyLogDto?> UpdateAsync(int id, UpdateDailyLogRequest request, CancellationToken ct = default)
    {
        var entity = await _repository.GetByIdAsync(id, ct);
        if (entity is null) return null;

        if (request.LogDate.HasValue) entity.LogDate = request.LogDate.Value;
        if (request.ActivityDescription is not null) entity.ActivityDescription = request.ActivityDescription;
        if (request.HoursWorked.HasValue) entity.HoursWorked = request.HoursWorked;
        if (request.ChallengesFaced is not null) entity.ChallengesFaced = request.ChallengesFaced;
        if (request.MentorFeedback is not null) entity.MentorFeedback = request.MentorFeedback;
        if (request.KpiScore.HasValue) entity.KpiScore = request.KpiScore;
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
