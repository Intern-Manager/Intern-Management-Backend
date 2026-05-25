using InternManagement.Application.Repositories;
using InternManagement.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace InternManagement.API.Endpoints;

public static class AuditLogEndpoints
{
    public static void MapAuditLogEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/audit-logs").WithTags("AuditLogs");

        group.MapGet("/", async (
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null,
            [FromQuery] string? logType = null,
            [FromServices] IAuditLogRepository repo = null!,
            CancellationToken ct = default) =>
        {
            var (items, total) = await repo.GetAllAsync(page, pageSize, search, logType, ct);
            return Results.Ok(new
            {
                items = items.Select(a => new
                {
                    a.AuditLogId,
                    a.UserId,
                    a.UserName,
                    a.Action,
                    a.EntityType,
                    a.EntityId,
                    a.Description,
                    a.IpAddress,
                    a.LogType,
                    a.CreatedAt
                }),
                total,
                page,
                pageSize
            });
        });
    }
}
