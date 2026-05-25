using InternManagement.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace InternManagement.API.Endpoints;

public static class CalendarEndpoints
{
    public static void MapCalendarEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/calendar").WithTags("Calendar Integration");

        group.MapGet("/status", async (int userId, ICalendarService calendarService, CancellationToken ct) =>
        {
            var isConnected = await calendarService.IsUserConnectedAsync(userId, ct);
            return Results.Ok(new { isConnected, message = isConnected ? "Calendar connected" : "Calendar not connected" });
        })
        .WithName("GetCalendarStatus")
        .WithSummary("Check if user's Google Calendar is connected")
        .Produces(200);

        group.MapGet("/connect", async (int userId, ICalendarService calendarService, CancellationToken ct) =>
        {
            var authUrl = await calendarService.GetAuthorizationUrlAsync(userId, ct);
            return Results.Ok(new { authUrl });
        })
        .WithName("ConnectCalendar")
        .WithSummary("Get Google OAuth authorization URL")
        .Produces(200);

        group.MapGet("/callback", async (string code, int userId, ICalendarService calendarService, CancellationToken ct) =>
        {
            var success = await calendarService.HandleOAuthCallbackAsync(code, userId, ct);
            if (success)
            {
                return Results.Ok(new { message = "Calendar connected successfully", redirectUrl = "/settings/calendar?success=true" });
            }
            return Results.BadRequest(new { message = "Failed to connect calendar" });
        })
        .WithName("CalendarCallback")
        .WithSummary("Handle Google OAuth callback")
        .Produces(200)
        .Produces(400);

        group.MapPost("/disconnect", async (int userId, ICalendarService calendarService, CancellationToken ct) =>
        {
            var success = await calendarService.DisconnectCalendarAsync(userId, ct);
            return success ? Results.Ok(new { message = "Calendar disconnected" }) : Results.BadRequest();
        })
        .WithName("DisconnectCalendar")
        .WithSummary("Disconnect Google Calendar")
        .Produces(200)
        .Produces(400);

        group.MapGet("/events", async (int userId, int maxResults, ICalendarService calendarService, CancellationToken ct) =>
        {
            var events = await calendarService.GetUpcomingEventsAsync(userId, maxResults, ct);
            return Results.Ok(events);
        })
        .WithName("GetCalendarEvents")
        .WithSummary("Get upcoming calendar events")
        .Produces(200);

        group.MapPost("/events/interview/{interviewId}", async (int createdByUserId, int interviewId, ICalendarService calendarService, CancellationToken ct) =>
        {
            var result = await calendarService.CreateInterviewEventAsync(createdByUserId, interviewId, ct);
            if (result == null)
            {
                return Results.NotFound(new { message = "Interview not found" });
            }
            return Results.Ok(result);
        })
        .WithName("CreateInterviewCalendarEvent")
        .WithSummary("Create calendar event for interview")
        .Produces(200)
        .Produces(404);

        group.MapPost("/events/training/{programId}", async (int createdByUserId, int programId, ICalendarService calendarService, CancellationToken ct) =>
        {
            var result = await calendarService.CreateTrainingEventAsync(createdByUserId, programId, ct);
            if (result == null)
            {
                return Results.NotFound(new { message = "Training program not found" });
            }
            return Results.Ok(result);
        })
        .WithName("CreateTrainingCalendarEvent")
        .WithSummary("Create calendar event for training program")
        .Produces(200)
        .Produces(404);

        group.MapPost("/events/task/{taskId}", async (int internUserId, int taskId, ICalendarService calendarService, CancellationToken ct) =>
        {
            var result = await calendarService.CreateTaskReminderEventAsync(internUserId, taskId, ct);
            if (result == null)
            {
                return Results.NotFound(new { message = "Task not found" });
            }
            return Results.Ok(result);
        })
        .WithName("CreateTaskReminderEvent")
        .WithSummary("Create calendar reminder for task deadline")
        .Produces(200)
        .Produces(404);

        group.MapDelete("/events/{eventId}", async (int userId, string eventId, ICalendarService calendarService, CancellationToken ct) =>
        {
            var success = await calendarService.DeleteEventAsync(userId, eventId, ct);
            return success ? Results.Ok() : Results.NotFound();
        })
        .WithName("DeleteCalendarEvent")
        .WithSummary("Delete a calendar event")
        .Produces(200)
        .Produces(404);
    }
}
