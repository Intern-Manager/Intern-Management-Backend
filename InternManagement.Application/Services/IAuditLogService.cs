namespace InternManagement.Application.Services;

public interface IAuditLogService
{
    Task LogAsync(AuditLogEntry entry, CancellationToken ct = default);
}

public class AuditLogEntry
{
    public int? UserId { get; set; }
    public string? UserName { get; set; }
    public string Action { get; set; } = string.Empty;
    public string EntityType { get; set; } = string.Empty;
    public int? EntityId { get; set; }
    public string? Description { get; set; }
    public string? IpAddress { get; set; }
    public string LogType { get; set; } = "Info";
}
