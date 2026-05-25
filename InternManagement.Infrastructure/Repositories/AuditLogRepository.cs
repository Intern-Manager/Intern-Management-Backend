using InternManagement.Application.Repositories;
using InternManagement.Domain.Entities;
using InternManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace InternManagement.Infrastructure.Repositories;

public class AuditLogRepository : IAuditLogRepository
{
    private readonly AppDbContext _db;

    public AuditLogRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task AddAsync(AuditLog log, CancellationToken ct)
    {
        _db.AuditLogs.Add(log);
        await _db.SaveChangesAsync(ct);
    }

    public async Task<(IEnumerable<AuditLog> Items, int Total)> GetAllAsync(
        int page, int pageSize, string? search, string? logType, CancellationToken ct)
    {
        var query = _db.AuditLogs.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(a => a.Action.Contains(search) || (a.Description != null && a.Description.Contains(search)));

        if (!string.IsNullOrWhiteSpace(logType))
            query = query.Where(a => a.LogType == logType);

        var total = await query.CountAsync(ct);
        var items = await query
            .OrderByDescending(a => a.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return (items, total);
    }
}
