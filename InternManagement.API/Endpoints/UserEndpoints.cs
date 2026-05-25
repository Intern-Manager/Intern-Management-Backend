using InternManagement.Application.DTOs;
using InternManagement.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace InternManagement.API.Endpoints;

public static class UserEndpoints
{
    public static void MapUserEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/users").WithTags("Users");

        group.MapGet("/", async (
            [FromQuery] string? search,
            [FromQuery] string? status,
            [FromQuery] int? roleId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            IUserService service = null!,
            CancellationToken ct = default) =>
        {
            var pagination = new PaginationRequest(page, pageSize);
            var filter = new UserFilter(search, status, roleId);
            return await service.GetAllAsync(pagination, filter, ct);
        });

        group.MapGet("/by-email", async (string email, IUserService service, CancellationToken ct) =>
        {
            var result = await service.GetByEmailAsync(email, ct);
            return result is null ? Results.NotFound() : Results.Ok(result);
        });

        group.MapGet("/{id:int}", async (int id, IUserService service, CancellationToken ct) =>
        {
            var result = await service.GetByIdAsync(id, ct);
            return result is null ? Results.NotFound() : Results.Ok(result);
        });

        group.MapPost("/", async (CreateUserRequest request, IUserService service, CancellationToken ct) =>
        {
            try
            {
                var result = await service.CreateAsync(request, ct);
                return Results.Created($"/api/users/{result!.UserId}", result);
            }
            catch (InvalidOperationException ex)
            {
                return Results.BadRequest(new { message = ex.Message });
            }
        });

        group.MapPut("/{id:int}", async (int id, UpdateUserRequest request, IUserService service, CancellationToken ct) =>
        {
            var result = await service.UpdateAsync(id, request, ct);
            return result is null ? Results.NotFound() : Results.Ok(result);
        });

        group.MapPatch("/{id:int}/avatar", async (int id, UpdateAvatarRequest request, IUserService service, CancellationToken ct) =>
        {
            if (string.IsNullOrWhiteSpace(request.Base64Image))
            {
                return Results.BadRequest(new { message = "Base64Image is required" });
            }
            var result = await service.UpdateAvatarAsync(id, request.Base64Image, ct);
            return result ? Results.Ok() : Results.NotFound();
        });

        group.MapDelete("/{id:int}", async (int id, IUserService service, CancellationToken ct) =>
            await service.DeleteAsync(id, ct) ? Results.NoContent() : Results.NotFound());
    }
}
