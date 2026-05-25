using InternManagement.Application.DTOs;
using InternManagement.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace InternManagement.API.Endpoints;

public static class CommunicationEndpoints
{
    public static void MapCommunicationEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/communications").WithTags("Communications");

        group.MapGet("/", async (
            [FromQuery] int? senderId,
            [FromQuery] int? receiverId,
            [FromQuery] bool? isRead,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 50,
            ICommunicationService service = null!,
            CancellationToken ct = default) =>
        {
            var pagination = new PaginationRequest(page, pageSize);
            var filter = new CommunicationFilter(senderId, receiverId, isRead);
            return await service.GetAllAsync(pagination, filter, ct);
        });

        group.MapGet("/conversations", async (
            HttpContext ctx,
            ICommunicationService service = null!,
            CancellationToken ct = default) =>
        {
            var userIdClaim = ctx.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                              ?? ctx.User.FindFirst("sub")?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
                return Results.Unauthorized();
            var result = await service.GetConversationsAsync(userId, ct);
            return Results.Ok(result);
        });

        group.MapGet("/my-messages", async (
            HttpContext ctx,
            ICommunicationService service = null!,
            CancellationToken ct = default) =>
        {
            var userIdClaim = ctx.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                              ?? ctx.User.FindFirst("sub")?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
                return Results.Unauthorized();

            var page = int.TryParse(ctx.Request.Query["page"], out var p) ? p : 1;
            var pageSize = int.TryParse(ctx.Request.Query["pageSize"], out var ps) ? ps : 50;
            var otherUserId = int.TryParse(ctx.Request.Query["otherUserId"], out var o) ? o : (int?)null;

            var pagination = new PaginationRequest(page, pageSize);
            var result = await service.GetMyMessagesAsync(userId, otherUserId, pagination, ct);
            return Results.Ok(result);
        });

        group.MapGet("/contacts", async (
            HttpContext ctx,
            IUserService userService = null!,
            CancellationToken ct = default) =>
        {
            var userIdClaim = ctx.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                              ?? ctx.User.FindFirst("sub")?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
                return Results.Unauthorized();
            var result = await userService.GetChatContactsAsync(userId, ct);
            return Results.Ok(result);
        });

        group.MapGet("/unread-count", async (
            HttpContext ctx,
            ICommunicationService service = null!,
            CancellationToken ct = default) =>
        {
            var userIdClaim = ctx.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                              ?? ctx.User.FindFirst("sub")?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
                return Results.Unauthorized();
            var result = await service.GetUnreadCountAsync(userId, ct);
            return Results.Ok(result);
        });

        group.MapGet("/{id:int}", async (int id, ICommunicationService service, CancellationToken ct) =>
        {
            var result = await service.GetByIdAsync(id, ct);
            return result is null ? Results.NotFound() : Results.Ok(result);
        });

        group.MapPost("/", async (
            HttpContext ctx,
            CreateCommunicationRequest request,
            ICommunicationService service = null!,
            CancellationToken ct = default) =>
        {
            var userIdClaim = ctx.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                              ?? ctx.User.FindFirst("sub")?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var senderId))
                return Results.Unauthorized();

            var secureRequest = request with { SenderId = senderId };
            var result = await service.CreateAsync(secureRequest, ct);
            return Results.Created("", result);
        });

        group.MapPut("/{id:int}", async (int id, UpdateCommunicationRequest request, ICommunicationService service, CancellationToken ct) =>
        {
            var result = await service.UpdateAsync(id, request, ct);
            return result is null ? Results.NotFound() : Results.Ok(result);
        });

        group.MapPut("/mark-read", async (
            HttpContext ctx,
            [FromBody] MarkMessagesReadRequest request,
            ICommunicationService service = null!,
            CancellationToken ct = default) =>
        {
            var userIdClaim = ctx.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                              ?? ctx.User.FindFirst("sub")?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
                return Results.Unauthorized();
            await service.MarkAsReadAsync(request.SenderId, userId, ct);
            return Results.NoContent();
        });

        group.MapDelete("/{id:int}", async (int id, ICommunicationService service, CancellationToken ct) =>
            await service.DeleteAsync(id, ct) ? Results.NoContent() : Results.NotFound());
    }
}

public record MarkMessagesReadRequest(int SenderId);
