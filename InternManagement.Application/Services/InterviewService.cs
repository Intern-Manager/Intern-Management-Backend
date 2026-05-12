using InternManagement.Application.DTOs;
using InternManagement.Application.Repositories;

namespace InternManagement.Application.Services;

public class InterviewService : IInterviewService
{
    private readonly IInterviewRepository _repository;

    public InterviewService(IInterviewRepository repository) => _repository = repository;

    public async Task<PaginatedResult<InterviewDto>> GetAllAsync(PaginationRequest pagination, InterviewFilter? filter = null, CancellationToken ct = default)
    {
        var count = await _repository.CountAsync(filter, ct);
        var items = await _repository.GetAllDtoAsync(pagination, filter, ct);
        return items.ToPaginatedResult(pagination, count);
    }

    public async Task<InterviewDetailDto?> GetByIdAsync(int id, CancellationToken ct = default)
        => await _repository.GetDetailByIdAsync(id, ct);

    public async Task<InterviewDto?> CreateAsync(CreateInterviewRequest request, CancellationToken ct = default)
    {
        var entity = request.ToEntity();
        entity.CreatedAt = DateTime.UtcNow;
        await _repository.AddAsync(entity, ct);
        return entity.ToDto();
    }

    public async Task<InterviewDto?> UpdateAsync(int id, UpdateInterviewRequest request, CancellationToken ct = default)
    {
        var entity = await _repository.GetByIdAsync(id, ct);
        if (entity is null) return null;

        if (request.ScheduledTime.HasValue) entity.ScheduledTime = request.ScheduledTime.Value;
        if (request.DurationMinutes.HasValue) entity.DurationMinutes = request.DurationMinutes.Value;
        if (request.InterviewType is not null) entity.InterviewType = request.InterviewType;
        if (request.MeetingLink is not null) entity.MeetingLink = request.MeetingLink;
        if (request.Location is not null) entity.Location = request.Location;
        if (request.Status is not null) entity.Status = request.Status;
        if (request.Feedback is not null) entity.Feedback = request.Feedback;
        if (request.Rating.HasValue) entity.Rating = request.Rating;
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
