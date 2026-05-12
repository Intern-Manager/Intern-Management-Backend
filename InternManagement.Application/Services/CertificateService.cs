using InternManagement.Application.DTOs;
using InternManagement.Application.Repositories;
using InternManagement.Domain.Entities;

namespace InternManagement.Application.Services;

/// <summary>
/// Certificate Service - giữ nguyên structure để đơn giản
/// Mẫu này dùng cho tất cả entities khác
/// </summary>
public class CertificateService : ICertificateService
{
    private readonly ICertificateRepository _repository;

    public CertificateService(ICertificateRepository repository) => _repository = repository;

    public async Task<PaginatedResult<CertificateDto>> GetAllAsync(
        PaginationRequest pagination,
        CertificateFilter? filter = null,
        CancellationToken ct = default)
    {
        var totalCount = await _repository.CountWithFilterAsync(filter, ct);
        var items = await _repository.GetAllPagedAsync(pagination, filter, ct);
        return items
            .Select(c => new CertificateDto(
                c.CertificateId, c.InternId, c.ProgramId, c.CertificateName,
                c.Description, c.IssuedDate, c.CertificateUrl, c.IssuedBy, c.CreatedAt))
            .ToPaginatedResult(pagination, totalCount);
    }

    public async Task<CertificateDetailDto?> GetByIdAsync(int id, CancellationToken ct = default)
        => await _repository.GetDetailByIdAsync(id, ct);

    public async Task<CertificateDto?> CreateAsync(CreateCertificateRequest request, CancellationToken ct = default)
    {
        var entity = new Certificate
        {
            InternId = request.InternId,
            ProgramId = request.ProgramId,
            CertificateName = request.CertificateName,
            Description = request.Description,
            IssuedDate = request.IssuedDate,
            CertificateUrl = request.CertificateUrl,
            IssuedBy = request.IssuedBy,
            CreatedAt = DateTime.UtcNow
        };
        await _repository.AddAsync(entity, ct);
        return new CertificateDto(
            entity.CertificateId, entity.InternId, entity.ProgramId, entity.CertificateName,
            entity.Description, entity.IssuedDate, entity.CertificateUrl, entity.IssuedBy, entity.CreatedAt);
    }

    public async Task<CertificateDto?> UpdateAsync(int id, UpdateCertificateRequest request, CancellationToken ct = default)
    {
        var entity = await _repository.GetByIdAsync(id, ct);
        if (entity is null) return null;

        if (request.CertificateName is not null) entity.CertificateName = request.CertificateName;
        if (request.Description is not null) entity.Description = request.Description;
        if (request.IssuedDate.HasValue) entity.IssuedDate = request.IssuedDate.Value;
        if (request.CertificateUrl is not null) entity.CertificateUrl = request.CertificateUrl;

        await _repository.UpdateAsync(entity, ct);
        return new CertificateDto(
            entity.CertificateId, entity.InternId, entity.ProgramId, entity.CertificateName,
            entity.Description, entity.IssuedDate, entity.CertificateUrl, entity.IssuedBy, entity.CreatedAt);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        if (!await _repository.ExistsAsync(id, ct)) return false;
        await _repository.DeleteAsync(id, ct);
        return true;
    }
}
