using InternManagement.API.Services;
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
            [FromQuery] string? search,
            [FromQuery] string? resourceType,
            [FromQuery] int? programId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            ILearningResourceService service = null!,
            CancellationToken ct = default) =>
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

        group.MapPost("/", async (CreateLearningResourceRequest request, HttpContext ctx, ILearningResourceService service, CancellationToken ct) =>
        {
            var userIdClaim = ctx.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                              ?? ctx.User.FindFirst("sub")?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
                return Results.Unauthorized();
            var result = await service.CreateAsync(request, userId, ct);
            return result is null ? Results.BadRequest() : Results.Created("", result);
        });

        group.MapPut("/{id:int}", async (int id, UpdateLearningResourceRequest request, ILearningResourceService service, CancellationToken ct) =>
        {
            var result = await service.UpdateAsync(id, request, ct);
            return result is null ? Results.NotFound() : Results.Ok(result);
        });

        group.MapDelete("/{id:int}", async (int id, ILearningResourceService service, CancellationToken ct) =>
            await service.DeleteAsync(id, ct) ? Results.NoContent() : Results.NotFound());

        group.MapPost("/upload/{programId:int}", async (
            int programId,
            HttpContext ctx,
            ILearningResourceService service,
            InternManagement.API.Services.CloudinaryService cloudinary,
            ILogger<Program> logger,
            CancellationToken ct) =>
        {
            try
            {
                logger.LogInformation("Upload request START: programId={ProgramId}, ContentLength={ContentLength}, ContentType={ContentType}, Headers={Headers}",
                    programId, ctx.Request.ContentLength, ctx.Request.ContentType, string.Join("; ", ctx.Request.Headers.Select(h => $"{h.Key}={string.Join(",", h.Value.ToArray())}")));
                var userIdClaim = ctx.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                                  ?? ctx.User.FindFirst("sub")?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
                    return Results.Unauthorized();

                logger.LogInformation("Attempting to read form files");
                var files = ctx.Request.Form.Files;
                logger.LogInformation("Form.Files count: {Count}", files.Count);
                if (files.Count == 0)
                    return Results.BadRequest(new { message = "No files provided" });

                const long maxSize = 500 * 1024 * 1024; // 500 MB
                var allowedExtensions = new[] { ".pdf", ".doc", ".docx", ".xls", ".xlsx", ".ppt", ".pptx", ".png", ".jpg", ".jpeg", ".gif", ".webp", ".mp4", ".mov", ".avi", ".webm", ".txt", ".zip" };

                var created = new List<object>();

                foreach (var file in files)
                {
                    logger.LogInformation("Processing file: {FileName}, Size={Size}, ContentType={ContentType}", file.FileName, file.Length, file.ContentType);
                    if (file.Length == 0) continue;
                    if (file.Length > maxSize)
                        return Results.BadRequest(new { message = $"File '{file.FileName}' exceeds 500 MB limit" });

                    var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
                    if (!allowedExtensions.Contains(ext))
                        return Results.BadRequest(new { message = $"File type '{ext}' is not allowed for '{file.FileName}'" });

                    try
                    {
                        var folder = file.ContentType.StartsWith("video/") ? "videos" : "documents";
                        await using var stream = file.OpenReadStream();
                        var url = await cloudinary.UploadFileAsync(stream, file.FileName, folder, ct);

                        var fileSizeMb = Math.Round((decimal)file.Length / 1024 / 1024, 2);
                        var resourceType = file.ContentType.StartsWith("video/") ? "Video"
                            : ext == ".pdf" || ext == ".doc" || ext == ".docx" || ext == ".txt" ? "Document"
                            : "Document";

                        var request = new CreateLearningResourceRequest(programId, file.FileName, null, url, resourceType, fileSizeMb);
                        var result = await service.CreateAsync(request, userId, ct);
                        if (result != null)
                            created.Add(new { result.ResourceId, result.Title, result.ResourceUrl, result.ResourceType });
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "Upload failed for file {FileName}", file.FileName);
                        return Results.BadRequest(new { message = $"Upload failed for '{file.FileName}': {ex.Message}" });
                    }
                }

                logger.LogInformation("Upload completed successfully: {Count} files", created.Count);
                return Results.Created("", new { message = $"Uploaded {created.Count} file(s)", resources = created });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unhandled exception in upload endpoint");
                return Results.BadRequest(new { message = $"Upload failed: {ex.Message}" });
            }
        }).DisableAntiforgery();
    }
}
