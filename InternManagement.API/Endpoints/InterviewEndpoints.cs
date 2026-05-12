using InternManagement.Application.DTOs;
using InternManagement.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace InternManagement.API.Endpoints;

public static class InterviewEndpoints
{
    public static void MapInterviewEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/interviews").WithTags("Interviews");

        group.MapGet("/", async (
            [FromQuery] int page,
            [FromQuery] int pageSize,
            [FromQuery] string? status,
            [FromQuery] int? campaignId,
            [FromQuery] int? internId,
            [FromQuery] int? interviewerId,
            [FromQuery] DateTime? fromDate,
            [FromQuery] DateTime? toDate,
            IInterviewService service,
            CancellationToken ct) =>
        {
            var pagination = new PaginationRequest(page, pageSize);
            var filter = new InterviewFilter(null, status, campaignId, internId, interviewerId, fromDate, toDate);
            return await service.GetAllAsync(pagination, filter, ct);
        });

        group.MapGet("/{id:int}", async (int id, IInterviewService service, CancellationToken ct) =>
        {
            var result = await service.GetByIdAsync(id, ct);
            return result is null ? Results.NotFound() : Results.Ok(result);
        });

        group.MapPost("/", async (CreateInterviewRequest request, IInterviewService service, CancellationToken ct) =>
            Results.Created("", await service.CreateAsync(request, ct)));

        group.MapPut("/{id:int}", async (int id, UpdateInterviewRequest request, IInterviewService service, CancellationToken ct) =>
        {
            var result = await service.UpdateAsync(id, request, ct);
            return result is null ? Results.NotFound() : Results.Ok(result);
        });

        group.MapDelete("/{id:int}", async (int id, IInterviewService service, CancellationToken ct) =>
            await service.DeleteAsync(id, ct) ? Results.NoContent() : Results.NotFound());
    }
}
