using InternManagement.Application.DTOs;
using InternManagement.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace InternManagement.API.Endpoints;

public static class AssessmentEndpoints
{
    public static void MapAssessmentEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/assessments").WithTags("Assessments");

        group.MapGet("/", async (
            [FromQuery] int page,
            [FromQuery] int pageSize,
            [FromQuery] string? assessmentType,
            [FromQuery] int? internId,
            [FromQuery] int? mentorId,
            [FromQuery] int? programId,
            [FromQuery] DateOnly? fromDate,
            [FromQuery] DateOnly? toDate,
            IAssessmentService service,
            CancellationToken ct) =>
        {
            var pagination = new PaginationRequest(page, pageSize);
            var filter = new AssessmentFilter(assessmentType, internId, mentorId, programId, fromDate, toDate);
            return await service.GetAllAsync(pagination, filter, ct);
        });

        group.MapGet("/{id:int}", async (int id, IAssessmentService service, CancellationToken ct) =>
        {
            var result = await service.GetByIdAsync(id, ct);
            return result is null ? Results.NotFound() : Results.Ok(result);
        });

        group.MapPost("/", async (CreateAssessmentRequest request, IAssessmentService service, CancellationToken ct) =>
            Results.Created("", await service.CreateAsync(request, ct)));

        group.MapPut("/{id:int}", async (int id, UpdateAssessmentRequest request, IAssessmentService service, CancellationToken ct) =>
        {
            var result = await service.UpdateAsync(id, request, ct);
            return result is null ? Results.NotFound() : Results.Ok(result);
        });

        group.MapDelete("/{id:int}", async (int id, IAssessmentService service, CancellationToken ct) =>
            await service.DeleteAsync(id, ct) ? Results.NoContent() : Results.NotFound());
    }
}
