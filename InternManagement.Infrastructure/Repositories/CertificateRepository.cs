using InternManagement.Application.DTOs;
using InternManagement.Application.Repositories;
using InternManagement.Domain.Entities;
using InternManagement.Infrastructure.Persistence;
using InternManagement.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace InternManagement.Infrastructure.Repositories;

/// <summary>
/// Certificate Repository - kế thừa BaseRepository
/// Giảm boilerplate code đáng kể
/// </summary>
public class CertificateRepository : BaseRepository<Certificate, int>, ICertificateRepository
{
    public CertificateRepository(AppDbContext context) : base(context) { }

    public async Task<CertificateDetailDto?> GetDetailByIdAsync(int id, CancellationToken ct = default)
    {
        var cert = await _dbSet.FirstOrDefaultAsync(c => c.CertificateId == id, ct);
        if (cert is null) return null;

        var internName = await _context.Users
            .Where(u => u.UserId == cert.InternId).Select(u => u.FullName).FirstOrDefaultAsync(ct);
        var programName = cert.ProgramId.HasValue
            ? await _context.TrainingPrograms.Where(p => p.ProgramId == cert.ProgramId).Select(p => p.ProgramName).FirstOrDefaultAsync(ct)
            : null;
        var issuedByName = await _context.Users
            .Where(u => u.UserId == cert.IssuedBy).Select(u => u.FullName).FirstOrDefaultAsync(ct);

        return new CertificateDetailDto(
            cert.CertificateId, cert.InternId, internName ?? "",
            cert.ProgramId, programName, cert.CertificateName, cert.Description,
            cert.IssuedDate, cert.CertificateUrl, cert.IssuedBy, issuedByName ?? "",
            cert.CreatedAt);
    }

    public async Task<IEnumerable<Certificate>> GetAllPagedAsync(
        PaginationRequest pagination,
        CertificateFilter? filter = null,
        CancellationToken ct = default)
    {
        var query = BuildFilterQuery(filter);
        return await query
            .OrderByDescending(c => c.IssuedDate)
            .Skip((pagination.Page - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .ToListAsync(ct);
    }

    public async Task<int> CountWithFilterAsync(CertificateFilter? filter, CancellationToken ct = default)
        => await BuildFilterQuery(filter).CountAsync(ct);

    private IQueryable<Certificate> BuildFilterQuery(CertificateFilter? filter)
    {
        var query = _dbSet.AsQueryable();
        if (!string.IsNullOrWhiteSpace(filter?.Search))
            query = query.Where(c => c.CertificateName.Contains(filter.Search));
        if (filter?.InternId.HasValue == true)
            query = query.Where(c => c.InternId == filter.InternId.Value);
        if (filter?.ProgramId.HasValue == true)
            query = query.Where(c => c.ProgramId == filter.ProgramId.Value);
        return query;
    }
}
