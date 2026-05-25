using InternManagement.Application.DTOs;
using InternManagement.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace InternManagement.API.Endpoints;

public static class InternProfileEndpoints
{
    public static void MapInternProfileEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/intern-profiles").WithTags("InternProfiles");

        group.MapGet("/", async (
            [FromQuery] string? search,
            [FromQuery] string? university,
            [FromQuery] string? major,
            [FromQuery] int? graduationYear,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            IInternProfileService service = null!,
            CancellationToken ct = default) =>
        {
            var pagination = new PaginationRequest(page, pageSize);
            var filter = new InternProfileFilter(search, university, major, graduationYear);
            return await service.GetAllAsync(pagination, filter, ct);
        });

        group.MapGet("/{id:int}", async (int id, IInternProfileService service, CancellationToken ct) =>
        {
            var result = await service.GetByIdAsync(id, ct);
            return result is null ? Results.NotFound() : Results.Ok(result);
        });

        group.MapPost("/", async (CreateInternProfileRequest request, IInternProfileService service, CancellationToken ct) =>
            Results.Created("", await service.CreateAsync(request, ct)));

        group.MapPut("/{id:int}", async (int id, UpdateInternProfileRequest request, IInternProfileService service, CancellationToken ct) =>
        {
            var result = await service.UpdateAsync(id, request, ct);
            return result is null ? Results.NotFound() : Results.Ok(result);
        });

        group.MapDelete("/{id:int}", async (int id, IInternProfileService service, CancellationToken ct) =>
            await service.DeleteAsync(id, ct) ? Results.NoContent() : Results.NotFound());
    }
}
