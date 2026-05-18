using InternManagement.Application.DTOs;
using InternManagement.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace InternManagement.API.Endpoints;

public static class MentorshipEndpoints
{
    public static void MapMentorshipEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/mentorships").WithTags("Mentorships");

        group.MapGet("/", async (
            [FromQuery] string? status,
            [FromQuery] int? mentorId,
            [FromQuery] int? internId,
            [FromQuery] int? programId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            IMentorshipService service = null!,
            CancellationToken ct = default) =>
        {
            var pagination = new PaginationRequest(page, pageSize);
            var filter = new MentorshipFilter(status, mentorId, internId, programId);
            return await service.GetAllAsync(pagination, filter, ct);
        });

        group.MapGet("/{id:int}", async (int id, IMentorshipService service, CancellationToken ct) =>
        {
            var result = await service.GetByIdAsync(id, ct);
            return result is null ? Results.NotFound() : Results.Ok(result);
        });

        group.MapPost("/", async (CreateMentorshipRequest request, IMentorshipService service, CancellationToken ct) =>
            Results.Created("", await service.CreateAsync(request, ct)));

        group.MapPut("/{id:int}", async (int id, UpdateMentorshipRequest request, IMentorshipService service, CancellationToken ct) =>
        {
            var result = await service.UpdateAsync(id, request, ct);
            return result is null ? Results.NotFound() : Results.Ok(result);
        });

        group.MapDelete("/{id:int}", async (int id, IMentorshipService service, CancellationToken ct) =>
            await service.DeleteAsync(id, ct) ? Results.NoContent() : Results.NotFound());
    }
}
