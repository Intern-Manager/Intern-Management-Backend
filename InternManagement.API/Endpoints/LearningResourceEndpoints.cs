using InternManagement.Application.DTOs;
using InternManagement.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace InternManagement.API.Endpoints;

public static class LearningResourceEndpoints
{
    public static void MapLearningResourceEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/learning-resources").WithTags("LearningResources");

        group.MapGet("/", async (
            [FromQuery] int page,
            [FromQuery] int pageSize,
            [FromQuery] string? search,
            [FromQuery] string? resourceType,
            [FromQuery] int? programId,
            ILearningResourceService service,
            CancellationToken ct) =>
        {
            var pagination = new PaginationRequest(page, pageSize);
            var filter = new LearningResourceFilter(search, resourceType, programId);
            return await service.GetAllAsync(pagination, filter, ct);
        });

        group.MapGet("/{id:int}", async (int id, ILearningResourceService service, CancellationToken ct) =>
        {
            var result = await service.GetByIdAsync(id, ct);
            return result is null ? Results.NotFound() : Results.Ok(result);
        });

        group.MapPost("/", async (CreateLearningResourceRequest request, ILearningResourceService service, CancellationToken ct) =>
            Results.Created("", await service.CreateAsync(request, ct)));

        group.MapPut("/{id:int}", async (int id, UpdateLearningResourceRequest request, ILearningResourceService service, CancellationToken ct) =>
        {
            var result = await service.UpdateAsync(id, request, ct);
            return result is null ? Results.NotFound() : Results.Ok(result);
        });

        group.MapDelete("/{id:int}", async (int id, ILearningResourceService service, CancellationToken ct) =>
            await service.DeleteAsync(id, ct) ? Results.NoContent() : Results.NotFound());
    }
}
