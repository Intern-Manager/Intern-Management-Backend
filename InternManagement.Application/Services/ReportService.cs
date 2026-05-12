using InternManagement.Application.DTOs;
using InternManagement.Application.Repositories;

namespace InternManagement.Application.Services;

public class ReportService : IReportService
{
    private readonly IReportRepository _repository;

    public ReportService(IReportRepository repository) => _repository = repository;

    public async Task<PaginatedResult<ReportDto>> GetAllAsync(PaginationRequest pagination, ReportFilter? filter = null, CancellationToken ct = default)
    {
        var count = await _repository.CountAsync(filter, ct);
        var items = await _repository.GetAllDtoAsync(pagination, filter, ct);
        return items.ToPaginatedResult(pagination, count);
    }

    public async Task<ReportDetailDto?> GetByIdAsync(int id, CancellationToken ct = default)
        => await _repository.GetDetailByIdAsync(id, ct);

    public async Task<ReportDto?> CreateAsync(CreateReportRequest request, CancellationToken ct = default)
    {
        var entity = request.ToEntity();
        entity.GeneratedAt = DateTime.UtcNow;
        await _repository.AddAsync(entity, ct);
        return entity.ToDto();
    }

    public async Task<ReportDto?> UpdateAsync(int id, UpdateReportRequest request, CancellationToken ct = default)
    {
        var entity = await _repository.GetByIdAsync(id, ct);
        if (entity is null) return null;

        if (request.ReportName is not null) entity.ReportName = request.ReportName;
        if (request.ReportType is not null) entity.ReportType = request.ReportType;
        if (request.Description is not null) entity.Description = request.Description;
        if (request.FileUrl is not null) entity.FileUrl = request.FileUrl;
        if (request.FileFormat is not null) entity.FileFormat = request.FileFormat;
        if (request.Parameters is not null) entity.Parameters = request.Parameters;

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
