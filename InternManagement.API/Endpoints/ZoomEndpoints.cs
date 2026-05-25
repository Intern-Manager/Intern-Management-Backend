using InternManagement.Application.Services;

namespace InternManagement.API.Endpoints;

public static class ZoomEndpoints
{
    public static void MapZoomEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/zoom").WithTags("Zoom Integration");

        group.MapPost("/meetings", async (ZoomMeetingRequest request, IZoomService zoomService, CancellationToken ct) =>
        {
            var meeting = await zoomService.CreateMeetingAsync(request, ct);
            if (meeting == null)
            {
                return Results.BadRequest(new { message = "Failed to create Zoom meeting" });
            }
            return Results.Ok(meeting);
        })
        .WithName("CreateZoomMeeting")
        .WithSummary("Create a new Zoom meeting")
        .Produces(200)
        .Produces(400);

        group.MapPost("/meetings/interview/{interviewId}", async (int interviewId, IZoomService zoomService, CancellationToken ct) =>
        {
            var meeting = await zoomService.CreateInterviewMeetingAsync(interviewId, ct);
            if (meeting == null)
            {
                return Results.NotFound(new { message = "Interview not found or Zoom API error" });
            }
            return Results.Ok(meeting);
        })
        .WithName("CreateInterviewZoomMeeting")
        .WithSummary("Create Zoom meeting for an interview and send invites")
        .Produces(200)
        .Produces(404);

        group.MapGet("/meetings/{meetingId}", async (string meetingId, IZoomService zoomService, CancellationToken ct) =>
        {
            var meeting = await zoomService.GetMeetingAsync(meetingId, ct);
            if (meeting == null)
            {
                return Results.NotFound(new { message = "Meeting not found" });
            }
            return Results.Ok(meeting);
        })
        .WithName("GetZoomMeeting")
        .WithSummary("Get Zoom meeting details")
        .Produces(200)
        .Produces(404);

        group.MapDelete("/meetings/{meetingId}", async (string meetingId, IZoomService zoomService, CancellationToken ct) =>
        {
            var success = await zoomService.DeleteMeetingAsync(meetingId, ct);
            return success ? Results.Ok() : Results.NotFound();
        })
        .WithName("DeleteZoomMeeting")
        .WithSummary("Delete a Zoom meeting")
        .Produces(200)
        .Produces(404);

        group.MapGet("/meetings/{meetingId}/recording", async (string meetingId, IZoomService zoomService, CancellationToken ct) =>
        {
            var recordingUrl = await zoomService.GetMeetingRecordingAsync(meetingId, ct);
            if (recordingUrl == null)
            {
                return Results.NotFound(new { message = "Recording not found or not available" });
            }
            return Results.Ok(new { recordingUrl });
        })
        .WithName("GetZoomMeetingRecording")
        .WithSummary("Get Zoom meeting recording URL")
        .Produces(200)
        .Produces(404);

        group.MapGet("/meetings/upcoming", async (int maxResults, IZoomService zoomService, CancellationToken ct) =>
        {
            var meetings = await zoomService.GetUpcomingMeetingsAsync(maxResults, ct);
            return Results.Ok(meetings);
        })
        .WithName("GetUpcomingZoomMeetings")
        .WithSummary("Get upcoming Zoom meetings")
        .Produces(200);
    }
}
