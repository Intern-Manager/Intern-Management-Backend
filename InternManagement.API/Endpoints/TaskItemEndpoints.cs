using InternManagement.Application.DTOs;
using InternManagement.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace InternManagement.API.Endpoints;

public static class TaskItemEndpoints
{
    public static void MapTaskItemEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/tasks").WithTags("Tasks");

        group.MapGet("/", async (
            [FromQuery] int page,
            [FromQuery] int pageSize,
            [FromQuery] string? search,
            [FromQuery] string? status,
            [FromQuery] string? priority,
            [FromQuery] int? internId,
            [FromQuery] int? assignedBy,
            [FromQuery] int? programId,
            ITaskItemService service,
            CancellationToken ct) =>
        {
            var pagination = new PaginationRequest(page, pageSize);
            var filter = new TaskItemFilter(search, status, priority, internId, assignedBy, programId);
            return await service.GetAllAsync(pagination, filter, ct);
        });

        group.MapGet("/{id:int}", async (int id, ITaskItemService service, CancellationToken ct) =>
        {
            var result = await service.GetByIdAsync(id, ct);
            return result is null ? Results.NotFound() : Results.Ok(result);
        });

        group.MapPost("/", async (CreateTaskItemRequest request, ITaskItemService service, CancellationToken ct) =>
            Results.Created("", await service.CreateAsync(request, ct)));

        group.MapPut("/{id:int}", async (int id, UpdateTaskItemRequest request, ITaskItemService service, CancellationToken ct) =>
        {
            var result = await service.UpdateAsync(id, request, ct);
            return result is null ? Results.NotFound() : Results.Ok(result);
        });

        group.MapDelete("/{id:int}", async (int id, ITaskItemService service, CancellationToken ct) =>
            await service.DeleteAsync(id, ct) ? Results.NoContent() : Results.NotFound());
    }
}
