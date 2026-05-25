using System.Net.Http.Headers;
using System.Text;
using InternManagement.Application.Repositories;
using InternManagement.Application.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace InternManagement.Infrastructure.Services;

public class ZoomService : IZoomService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<ZoomService> _logger;
    private readonly IInterviewRepository _interviewRepository;
    private readonly IUserRepository _userRepository;
    private readonly IEmailService _emailService;
    private readonly string _accountId;
    private readonly string _clientId;
    private readonly string _clientSecret;
    private string? _accessToken;
    private DateTime _tokenExpiry;

    public ZoomService(
        IConfiguration configuration,
        ILogger<ZoomService> logger,
        IInterviewRepository interviewRepository,
        IUserRepository userRepository,
        IEmailService emailService)
    {
        _configuration = configuration;
        _logger = logger;
        _interviewRepository = interviewRepository;
        _userRepository = userRepository;
        _emailService = emailService;
        _accountId = configuration["Zoom:AccountId"] ?? throw new ArgumentNullException("Zoom:AccountId");
        _clientId = configuration["Zoom:ClientId"] ?? throw new ArgumentNullException("Zoom:ClientId");
        _clientSecret = configuration["Zoom:ClientSecret"] ?? throw new ArgumentNullException("Zoom:ClientSecret");
        _tokenExpiry = DateTime.MinValue;
    }

    public async Task<ZoomMeetingResponse?> CreateMeetingAsync(ZoomMeetingRequest request, CancellationToken ct = default)
    {
        try
        {
            var token = await GetAccessTokenAsync(ct);
            if (string.IsNullOrEmpty(token))
            {
                _logger.LogError("Failed to get Zoom access token");
                return null;
            }

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var zoomRequest = new
            {
                topic = request.Topic,
                type = 2, // Scheduled meeting
                start_time = request.StartTime.ToString("yyyy-MM-ddTHH:mm:ss"),
                duration = request.DurationMinutes,
                timezone = request.TimeZone,
                agenda = request.Agenda,
                settings = new
                {
                    host_video = true,
                    participant_video = true,
                    join_before_host = false,
                    mute_upon_entry = true,
                    auto_recording = request.AutoRecord ? "cloud" : "none",
                    meeting_authentication = false
                }
            };

            var jsonContent = JsonConvert.SerializeObject(zoomRequest);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var response = await client.PostAsync("https://api.zoom.us/v2/users/me/meetings", content, ct);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync(ct);
                _logger.LogError("Failed to create Zoom meeting. Status: {Status}, Error: {Error}",
                    response.StatusCode, errorContent);
                return null;
            }

            var responseContent = await response.Content.ReadAsStringAsync(ct);
            var meetingData = JsonConvert.DeserializeObject<dynamic>(responseContent);

            if (meetingData == null)
            {
                _logger.LogError("Failed to parse Zoom meeting response");
                return null;
            }

            var result = new ZoomMeetingResponse
            {
                MeetingId = meetingData.id.ToString(),
                Topic = meetingData.topic.ToString(),
                JoinUrl = meetingData.join_url.ToString(),
                StartUrl = meetingData.start_url.ToString(),
                Password = meetingData.password?.ToString(),
                StartTime = DateTime.Parse(meetingData.start_time.ToString()),
                Duration = (int)meetingData.duration,
                CreatedAt = DateTime.UtcNow
            };

            _logger.LogInformation("Zoom meeting created: {MeetingId} - {Topic}", result.MeetingId, result.Topic);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating Zoom meeting");
            return null;
        }
    }

    public async Task<ZoomMeetingResponse?> CreateInterviewMeetingAsync(int interviewId, CancellationToken ct = default)
    {
        var interview = await _interviewRepository.GetByIdAsync(interviewId, ct);
        if (interview == null)
        {
            _logger.LogWarning("Interview {InterviewId} not found", interviewId);
            return null;
        }

        var interviewUser = await _userRepository.GetByIdAsync(interview.InternId ?? 0, ct);
        var interviewerUser = await _userRepository.GetByIdAsync(interview.InterviewerId, ct);

        var participantEmails = new List<string>();
        if (interviewUser?.Email != null) participantEmails.Add(interviewUser.Email);
        if (interviewerUser?.Email != null) participantEmails.Add(interviewerUser.Email);

        var request = new ZoomMeetingRequest
        {
            Topic = $"Interview: {interviewUser?.FullName ?? "Candidate"}",
            StartTime = interview.ScheduledTime,
            DurationMinutes = interview.DurationMinutes,
            Agenda = $"Interview Type: {interview.InterviewType}\n\nInterview with {interviewUser?.FullName}",
            ParticipantEmails = participantEmails,
            AutoRecord = true
        };

        var meeting = await CreateMeetingAsync(request, ct);
        if (meeting == null) return null;

        // Send email notifications to participants
        foreach (var email in participantEmails)
        {
            var name = email == interviewUser?.Email ? interviewUser?.FullName : interviewerUser?.FullName;
            if (!string.IsNullOrEmpty(name))
            {
                await _emailService.SendInterviewInvitationAsync(new InterviewEmailData
                {
                    CandidateName = name,
                    CandidateEmail = email,
                    Position = "Internship Interview",
                    InterviewDate = interview.ScheduledTime,
                    StartTime = interview.ScheduledTime.ToString("HH:mm"),
                    Duration = $"{interview.DurationMinutes} minutes",
                    InterviewType = interview.InterviewType,
                    MeetingLink = meeting.JoinUrl,
                    InterviewerName = interviewerUser?.FullName ?? "HR Team"
                }, ct);
            }
        }

        return meeting;
    }

    public async Task<ZoomMeetingResponse?> GetMeetingAsync(string meetingId, CancellationToken ct = default)
    {
        try
        {
            var token = await GetAccessTokenAsync(ct);
            if (string.IsNullOrEmpty(token)) return null;

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await client.GetAsync($"https://api.zoom.us/v2/meetings/{meetingId}", ct);
            if (!response.IsSuccessStatusCode) return null;

            var content = await response.Content.ReadAsStringAsync(ct);
            var meetingData = JsonConvert.DeserializeObject<dynamic>(content);

            if (meetingData == null) return null;

            return new ZoomMeetingResponse
            {
                MeetingId = meetingData.id.ToString(),
                Topic = meetingData.topic.ToString(),
                JoinUrl = meetingData.join_url.ToString(),
                StartUrl = meetingData.start_url.ToString(),
                Password = meetingData.password?.ToString(),
                StartTime = DateTime.Parse(meetingData.start_time.ToString()),
                Duration = (int)meetingData.duration,
                CreatedAt = DateTime.UtcNow
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting Zoom meeting {MeetingId}", meetingId);
            return null;
        }
    }

    public async Task<bool> DeleteMeetingAsync(string meetingId, CancellationToken ct = default)
    {
        try
        {
            var token = await GetAccessTokenAsync(ct);
            if (string.IsNullOrEmpty(token)) return false;

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await client.DeleteAsync($"https://api.zoom.us/v2/meetings/{meetingId}", ct);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting Zoom meeting {MeetingId}", meetingId);
            return false;
        }
    }

    public async Task<string?> GetMeetingRecordingAsync(string meetingId, CancellationToken ct = default)
    {
        try
        {
            var token = await GetAccessTokenAsync(ct);
            if (string.IsNullOrEmpty(token)) return null;

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await client.GetAsync($"https://api.zoom.us/v2/meetings/{meetingId}/recordings", ct);
            if (!response.IsSuccessStatusCode) return null;

            var content = await response.Content.ReadAsStringAsync(ct);
            var recordingData = JsonConvert.DeserializeObject<dynamic>(content);

            if (recordingData?.recording_files != null)
            {
                foreach (var file in recordingData.recording_files)
                {
                    if (file.file_type == "MP4")
                    {
                        return file.download_url?.ToString();
                    }
                }
            }

            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting recording for meeting {MeetingId}", meetingId);
            return null;
        }
    }

    public async Task<IEnumerable<ZoomMeetingResponse>> GetUpcomingMeetingsAsync(int maxResults = 10, CancellationToken ct = default)
    {
        try
        {
            var token = await GetAccessTokenAsync(ct);
            if (string.IsNullOrEmpty(token)) return Enumerable.Empty<ZoomMeetingResponse>();

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await client.GetAsync($"https://api.zoom.us/v2/users/me/meetings?type=upcoming&page_size={maxResults}", ct);
            if (!response.IsSuccessStatusCode) return Enumerable.Empty<ZoomMeetingResponse>();

            var content = await response.Content.ReadAsStringAsync(ct);
            var meetingsData = JsonConvert.DeserializeObject<dynamic>(content);

            if (meetingsData?.meetings == null) return Enumerable.Empty<ZoomMeetingResponse>();

            var meetings = new List<ZoomMeetingResponse>();
            foreach (var meeting in meetingsData.meetings)
            {
                meetings.Add(new ZoomMeetingResponse
                {
                    MeetingId = meeting.id.ToString(),
                    Topic = meeting.topic.ToString(),
                    JoinUrl = meeting.join_url.ToString(),
                    StartUrl = meeting.start_url.ToString(),
                    Password = meeting.password?.ToString(),
                    StartTime = DateTime.Parse(meeting.start_time.ToString()),
                    Duration = (int)meeting.duration,
                    CreatedAt = DateTime.UtcNow
                });
            }

            return meetings;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting upcoming Zoom meetings");
            return Enumerable.Empty<ZoomMeetingResponse>();
        }
    }

    private async Task<string?> GetAccessTokenAsync(CancellationToken ct)
    {
        if (!string.IsNullOrEmpty(_accessToken) && DateTime.UtcNow < _tokenExpiry)
        {
            return _accessToken;
        }

        try
        {
            using var client = new HttpClient();

            var credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_clientId}:{_clientSecret}"));

            var tokenRequest = new Dictionary<string, string>
            {
                ["grant_type"] = "account_credentials",
                ["account_id"] = _accountId
            };

            var content = new FormUrlEncodedContent(tokenRequest);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);

            var response = await client.PostAsync("https://zoom.us/oauth/token", content, ct);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync(ct);
                _logger.LogError("Failed to get Zoom access token. Status: {Status}, Error: {Error}",
                    response.StatusCode, errorContent);
                return null;
            }

            var responseContent = await response.Content.ReadAsStringAsync(ct);
            var tokenData = JsonConvert.DeserializeObject<dynamic>(responseContent);

            if (tokenData == null) return null;

            _accessToken = tokenData.access_token.ToString();
            _tokenExpiry = DateTime.UtcNow.AddSeconds((int)tokenData.expires_in - 60); // Refresh 1 min early

            return _accessToken;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting Zoom access token");
            return null;
        }
    }
}
