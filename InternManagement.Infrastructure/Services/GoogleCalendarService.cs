using System.Net.Http.Headers;
using System.Text;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using InternManagement.Application.Repositories;
using InternManagement.Application.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace InternManagement.Infrastructure.Services;

public class GoogleCalendarService : ICalendarService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<GoogleCalendarService> _logger;
    private readonly IUserRepository _userRepository;
    private readonly string _clientId;
    private readonly string _clientSecret;
    private readonly string _redirectUri;

    public GoogleCalendarService(
        IConfiguration configuration,
        ILogger<GoogleCalendarService> logger,
        IUserRepository userRepository)
    {
        _configuration = configuration;
        _logger = logger;
        _userRepository = userRepository;
        _clientId = configuration["GoogleCalendar:ClientId"] ?? throw new ArgumentNullException("GoogleCalendar:ClientId");
        _clientSecret = configuration["GoogleCalendar:ClientSecret"] ?? throw new ArgumentNullException("GoogleCalendar:ClientSecret");
        _redirectUri = configuration["GoogleCalendar:RedirectUri"] ?? throw new ArgumentNullException("GoogleCalendar:RedirectUri");
    }

    public Task<bool> IsUserConnectedAsync(int userId, CancellationToken ct = default)
    {
        // TODO: Check if user has connected Google Calendar
        return Task.FromResult(false);
    }

    public Task<string> GetAuthorizationUrlAsync(int userId, CancellationToken ct = default)
    {
        var scopes = new[]
        {
            Google.Apis.Calendar.v3.CalendarService.Scope.Calendar,
            Google.Apis.Calendar.v3.CalendarService.Scope.CalendarEvents
        };

        var state = Convert.ToBase64String(Encoding.UTF8.GetBytes($"userId={userId}"));

        var authorizationUrl = $"https://accounts.google.com/o/oauth2/v2/auth?" +
            $"client_id={Uri.EscapeDataString(_clientId)}&" +
            $"redirect_uri={Uri.EscapeDataString(_redirectUri)}&" +
            $"response_type=code&" +
            $"scope={Uri.EscapeDataString(string.Join(" ", scopes))}&" +
            $"access_type=offline&" +
            $"prompt=consent&" +
            $"state={state}";

        return Task.FromResult(authorizationUrl);
    }

    public Task<bool> HandleOAuthCallbackAsync(string code, int userId, CancellationToken ct = default)
    {
        // TODO: Exchange code for tokens and store refresh token
        _logger.LogInformation("OAuth callback received for user {UserId}", userId);
        return Task.FromResult(true);
    }

    public Task<bool> DisconnectCalendarAsync(int userId, CancellationToken ct = default)
    {
        // TODO: Remove refresh token from user profile
        _logger.LogInformation("Calendar disconnected for user {UserId}", userId);
        return Task.FromResult(true);
    }

    public async Task<CalendarEventResponse?> CreateEventAsync(int userId, CalendarEventRequest request, CancellationToken ct = default)
    {
        try
        {
            var user = await _userRepository.GetByIdAsync(userId, ct);
            if (user == null)
            {
                _logger.LogWarning("User {UserId} not found", userId);
                return null;
            }

            // For demo purposes, return a mock response
            // In production, you would use the Google Calendar API with OAuth tokens
            var eventId = Guid.NewGuid().ToString("N");
            
            _logger.LogInformation("Calendar event created for user {UserId}: {Summary}", userId, request.Summary);

            return new CalendarEventResponse
            {
                EventId = eventId,
                HtmlLink = $"https://calendar.google.com/calendar/event?eid={eventId}",
                MeetingLink = request.Location,
                CreatedAt = DateTime.UtcNow
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating calendar event for user {UserId}", userId);
            return null;
        }
    }

    public Task<CalendarEventResponse?> CreateInterviewEventAsync(int createdByUserId, int interviewId, CancellationToken ct = default)
    {
        var request = new CalendarEventRequest
        {
            Summary = "Interview",
            Description = "Interview scheduled via Intern Management System",
            StartTime = DateTime.UtcNow.AddDays(1),
            EndTime = DateTime.UtcNow.AddDays(1).AddHours(1),
            SendNotifications = true
        };

        return CreateEventAsync(createdByUserId, request, ct);
    }

    public Task<CalendarEventResponse?> CreateTrainingEventAsync(int createdByUserId, int programId, CancellationToken ct = default)
    {
        var request = new CalendarEventRequest
        {
            Summary = "Training Program",
            Description = "Training session scheduled via Intern Management System",
            StartTime = DateTime.UtcNow.AddDays(3),
            EndTime = DateTime.UtcNow.AddDays(3).AddHours(2),
            SendNotifications = true
        };

        return CreateEventAsync(createdByUserId, request, ct);
    }

    public Task<CalendarEventResponse?> CreateTaskReminderEventAsync(int internUserId, int taskId, CancellationToken ct = default)
    {
        var request = new CalendarEventRequest
        {
            Summary = "Task Deadline",
            Description = "Task deadline reminder from Intern Management System",
            StartTime = DateTime.UtcNow.AddDays(1),
            EndTime = DateTime.UtcNow.AddDays(1).AddMinutes(30),
            SendNotifications = true
        };

        return CreateEventAsync(internUserId, request, ct);
    }

    public Task<bool> UpdateEventAsync(int userId, string eventId, CalendarEventRequest request, CancellationToken ct = default)
    {
        _logger.LogInformation("Calendar event {EventId} updated for user {UserId}", eventId, userId);
        return Task.FromResult(true);
    }

    public Task<bool> DeleteEventAsync(int userId, string eventId, CancellationToken ct = default)
    {
        _logger.LogInformation("Calendar event {EventId} deleted for user {UserId}", eventId, userId);
        return Task.FromResult(true);
    }

    public Task<IEnumerable<CalendarEventResponse>> GetUpcomingEventsAsync(int userId, int maxResults = 10, CancellationToken ct = default)
    {
        // TODO: Return actual events from Google Calendar
        return Task.FromResult(Enumerable.Empty<CalendarEventResponse>());
    }
}
