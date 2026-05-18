using InternManagement.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace InternManagement.API.Endpoints;

public static class UploadEndpoints
{
    public static void MapUploadEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/uploads").WithTags("Uploads");

        group.MapPost("/file", UploadFile).DisableAntiforgery();
    }

    private static async Task<IResult> UploadFile(
        IFormFile file,
        [FromQuery] string folder,
        CloudinaryService cloudinary,
        CancellationToken ct)
    {
        if (file == null || file.Length == 0)
            return Results.BadRequest(new { message = "No file provided" });

        const long maxSize = 10 * 1024 * 1024; // 10 MB
        if (file.Length > maxSize)
            return Results.BadRequest(new { message = "File size exceeds 10 MB limit" });

        var allowedExtensions = new[] { ".pdf", ".doc", ".docx", ".png", ".jpg", ".jpeg", ".gif", ".webp" };
        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!allowedExtensions.Contains(ext))
            return Results.BadRequest(new { message = $"File type {ext} is not allowed. Allowed: {string.Join(", ", allowedExtensions)}" });

        try
        {
            await using var stream = file.OpenReadStream();
            var url = await cloudinary.UploadFileAsync(stream, file.FileName, folder ?? "general", ct);
            return Results.Ok(new { url });
        }
        catch (Exception ex)
        {
            return Results.BadRequest(new { message = $"Upload failed: {ex.Message}" });
        }
    }
}
