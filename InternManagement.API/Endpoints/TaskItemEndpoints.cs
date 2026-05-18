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
            [FromQuery] string? search,
            [FromQuery] string? status,
            [FromQuery] string? priority,
            [FromQuery] int? internId,
            [FromQuery] int? assignedBy,
            [FromQuery] int? programId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            ITaskItemService service = null!,
            CancellationToken ct = default) =>
        {
            var pagination = new PaginationRequest(page, pageSize);
            var filter = new TaskItemFilter(search, status, priority, internId, assignedBy, programId);
            return await service.GetAllAsync(pagination, filter, ct);
        });

        group.MapGet("/intern/{internId:int}", async (int internId, [FromQuery] string? search, [FromQuery] string? status, [FromQuery] string? priority, [FromQuery] int? programId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10, ITaskItemService service = null!, CancellationToken ct = default) =>
        {
            var pagination = new PaginationRequest(page, pageSize);
            var filter = new TaskItemFilter(search, status, priority, internId, null, programId);
            return await service.GetAllAsync(pagination, filter, ct);
        });

        group.MapGet("/mentor/{mentorId:int}", async (int mentorId, [FromQuery] string? search, [FromQuery] string? status, [FromQuery] string? priority, [FromQuery] int? programId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10, ITaskItemService service = null!, CancellationToken ct = default) =>
        {
            var pagination = new PaginationRequest(page, pageSize);
            var filter = new TaskItemFilter(search, status, priority, null, mentorId, programId);
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

        // Submissions
        group.MapGet("/{taskId:int}/submissions", async (int taskId, ITaskSubmissionService service, CancellationToken ct) =>
            await service.GetByTaskIdAsync(taskId, ct));

        group.MapPost("/{taskId:int}/submit", async (int taskId, CreateTaskSubmissionRequest request, ITaskSubmissionService service, CancellationToken ct) =>
        {
            var submission = request with { TaskId = taskId };
            return Results.Created("", await service.CreateAsync(submission, ct));
        });

        group.MapPut("/submissions/{id:int}/grade", async (int id, [FromBody] GradeSubmissionRequest request, ITaskSubmissionService service, CancellationToken ct) =>
        {
            // Get gradedBy from auth context (simplified - in real app, get from JWT)
            var gradedBy = 1;
            var result = await service.GradeAsync(id, gradedBy, request, ct);
            return result is null ? Results.NotFound() : Results.Ok(result);
        });
    }
}
