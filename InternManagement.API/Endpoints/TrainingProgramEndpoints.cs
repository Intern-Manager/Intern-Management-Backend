using InternManagement.Application.DTOs;
using InternManagement.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace InternManagement.API.Endpoints;

public static class TrainingProgramEndpoints
{
    public static void MapTrainingProgramEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/training-programs").WithTags("TrainingPrograms");

        group.MapGet("/", async (
            [FromQuery] int page,
            [FromQuery] int pageSize,
            [FromQuery] string? search,
            [FromQuery] string? status,
            [FromQuery] int? coordinatorId,
            ITrainingProgramService service,
            CancellationToken ct) =>
        {
            var pagination = new PaginationRequest(page, pageSize);
            var filter = new TrainingProgramFilter(search, status, coordinatorId);
            return await service.GetAllAsync(pagination, filter, ct);
        });

        group.MapGet("/{id:int}", async (int id, ITrainingProgramService service, CancellationToken ct) =>
        {
            var result = await service.GetByIdAsync(id, ct);
            return result is null ? Results.NotFound() : Results.Ok(result);
        });

        group.MapPost("/", async (CreateTrainingProgramRequest request, ITrainingProgramService service, CancellationToken ct) =>
            Results.Created("", await service.CreateAsync(request, ct)));

        group.MapPut("/{id:int}", async (int id, UpdateTrainingProgramRequest request, ITrainingProgramService service, CancellationToken ct) =>
        {
            var result = await service.UpdateAsync(id, request, ct);
            return result is null ? Results.NotFound() : Results.Ok(result);
        });

        group.MapDelete("/{id:int}", async (int id, ITrainingProgramService service, CancellationToken ct) =>
            await service.DeleteAsync(id, ct) ? Results.NoContent() : Results.NotFound());
    }
}
