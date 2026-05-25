using InternManagement.Application.DTOs;
using InternManagement.Application.Repositories;

namespace InternManagement.Application.Services;

public class AttendanceService : IAttendanceService
{
    private readonly IAttendanceRepository _repository;

    public AttendanceService(IAttendanceRepository repository) => _repository = repository;

    public async Task<PaginatedResult<AttendanceDto>> GetAllAsync(PaginationRequest pagination, AttendanceFilter? filter = null, CancellationToken ct = default)
    {
        var count = await _repository.CountAsync(filter, ct);
        var items = await _repository.GetAllDtoAsync(pagination, filter, ct);
        return items.ToPaginatedResult(pagination, count);
    }

    public async Task<AttendanceDetailDto?> GetByIdAsync(int id, CancellationToken ct = default)
        => await _repository.GetDetailByIdAsync(id, ct);

    public async Task<AttendanceDto?> CreateAsync(CreateAttendanceRequest request, CancellationToken ct = default)
    {
        var entity = request.ToEntity();
        entity.CreatedAt = DateTime.UtcNow;
        await _repository.AddAsync(entity, ct);
        var user = await _repository.GetUserNameAsync(request.InternId, ct);
        return entity.ToDto(user);
    }

    public async Task<AttendanceDto?> UpdateAsync(int id, UpdateAttendanceRequest request, CancellationToken ct = default)
    {
        var entity = await _repository.GetByIdAsync(id, ct);
        if (entity is null) return null;

        if (request.CheckInTime.HasValue) entity.CheckInTime = request.CheckInTime;
        if (request.CheckOutTime.HasValue) entity.CheckOutTime = request.CheckOutTime;
        if (request.Status is not null) entity.Status = request.Status;
        if (request.Notes is not null) entity.Notes = request.Notes;
        if (request.ApprovedBy.HasValue) entity.ApprovedBy = request.ApprovedBy;

        await _repository.UpdateAsync(entity, ct);
        var user = await _repository.GetUserNameAsync(entity.InternId, ct);
        return entity.ToDto(user);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        if (!await _repository.ExistsAsync(id, ct)) return false;
        await _repository.DeleteAsync(id, ct);
        return true;
    }
}
