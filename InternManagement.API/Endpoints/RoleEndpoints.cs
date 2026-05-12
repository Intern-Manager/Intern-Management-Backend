using InternManagement.Application.DTOs;
using InternManagement.Application.Services;

namespace InternManagement.API.Endpoints;

public static class RoleEndpoints
{
    public static void MapRoleEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/roles").WithTags("Roles");

        group.MapGet("/", async (IRoleService service, CancellationToken ct) =>
            await service.GetAllAsync(ct));

        group.MapGet("/{id:int}", async (int id, IRoleService service, CancellationToken ct) =>
        {
            var result = await service.GetByIdAsync(id, ct);
            return result is null ? Results.NotFound() : Results.Ok(result);
        });

        group.MapPost("/", async (CreateRoleRequest request, IRoleService service, CancellationToken ct) =>
            Results.Created("", await service.CreateAsync(request, ct)));

        group.MapPut("/{id:int}", async (int id, UpdateRoleRequest request, IRoleService service, CancellationToken ct) =>
        {
            var result = await service.UpdateAsync(id, request, ct);
            return result is null ? Results.NotFound() : Results.Ok(result);
        });

        group.MapDelete("/{id:int}", async (int id, IRoleService service, CancellationToken ct) =>
            await service.DeleteAsync(id, ct) ? Results.NoContent() : Results.NotFound());
    }
}
