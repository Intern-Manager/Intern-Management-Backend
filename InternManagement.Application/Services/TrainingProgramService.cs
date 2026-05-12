using InternManagement.Application.DTOs;
using InternManagement.Application.Repositories;

namespace InternManagement.Application.Services;

public class TrainingProgramService : ITrainingProgramService
{
    private readonly ITrainingProgramRepository _repository;

    public TrainingProgramService(ITrainingProgramRepository repository) => _repository = repository;

    public async Task<PaginatedResult<TrainingProgramDto>> GetAllAsync(PaginationRequest pagination, TrainingProgramFilter? filter = null, CancellationToken ct = default)
    {
        var count = await _repository.CountAsync(filter, ct);
        var items = await _repository.GetAllDtoAsync(pagination, filter, ct);
        return items.ToPaginatedResult(pagination, count);
    }

    public async Task<TrainingProgramDetailDto?> GetByIdAsync(int id, CancellationToken ct = default)
        => await _repository.GetDetailByIdAsync(id, ct);

    public async Task<TrainingProgramDto?> CreateAsync(CreateTrainingProgramRequest request, CancellationToken ct = default)
    {
        var entity = request.ToEntity();
        entity.CreatedAt = DateTime.UtcNow;
        await _repository.AddAsync(entity, ct);
        return entity.ToDto();
    }

    public async Task<TrainingProgramDto?> UpdateAsync(int id, UpdateTrainingProgramRequest request, CancellationToken ct = default)
    {
        var entity = await _repository.GetByIdAsync(id, ct);
        if (entity is null) return null;

        if (request.ProgramName is not null) entity.ProgramName = request.ProgramName;
        if (request.Description is not null) entity.Description = request.Description;
        if (request.Objectives is not null) entity.Objectives = request.Objectives;
        if (request.DurationWeeks.HasValue) entity.DurationWeeks = request.DurationWeeks;
        if (request.StartDate.HasValue) entity.StartDate = request.StartDate;
        if (request.EndDate.HasValue) entity.EndDate = request.EndDate;
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
