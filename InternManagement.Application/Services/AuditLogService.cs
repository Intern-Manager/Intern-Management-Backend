using InternManagement.Application.Repositories;
using InternManagement.Domain.Entities;

namespace InternManagement.Application.Services;

public class AuditLogService : IAuditLogService
{
    private readonly IAuditLogRepository _repository;

    public AuditLogService(IAuditLogRepository repository)
    {
        _repository = repository;
    }

    public async Task LogAsync(AuditLogEntry entry, CancellationToken ct = default)
    {
        var log = new AuditLog
        {
            UserId = entry.UserId,
            UserName = entry.UserName,
            Action = entry.Action,
            EntityType = entry.EntityType,
            EntityId = entry.EntityId,
            Description = entry.Description,
            IpAddress = entry.IpAddress,
            LogType = entry.LogType,
            CreatedAt = DateTime.UtcNow
        };

        await _repository.AddAsync(log, ct);
    }
}
