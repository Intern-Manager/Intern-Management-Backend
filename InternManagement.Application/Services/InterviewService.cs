using InternManagement.Application.DTOs;
using InternManagement.Application.Repositories;

namespace InternManagement.Application.Services;

public class InterviewService : IInterviewService
{
    private readonly IInterviewRepository _repository;
    private readonly ICampaignApplicationRepository _appRepository;
    private readonly INotificationService _notificationService;
    private readonly IUserRepository _userRepository;
    private readonly IZoomService _zoomService;

    public InterviewService(
        IInterviewRepository repository,
        ICampaignApplicationRepository appRepository,
        INotificationService notificationService,
        IUserRepository userRepository,
        IZoomService zoomService)
    {
        _repository = repository;
        _appRepository = appRepository;
        _notificationService = notificationService;
        _userRepository = userRepository;
        _zoomService = zoomService;
    }

    private async Task NotifyUserAsync(int userId, string subject, string content, string category, int? relatedId, string? relatedType, CancellationToken ct)
    {
        try
        {
            await _notificationService.CreateAsync(new CreateNotificationRequest(
                userId, "In-App", category, subject, content, relatedId, relatedType), ct);
        }
        catch { /* don't fail the main operation if notification fails */ }
    }

    private async Task<int?> GetUserIdByEmailAsync(string email, CancellationToken ct)
    {
        var user = await _userRepository.FindByEmailAsync(email, ct);
        return user?.UserId;
    }

    public async Task<PaginatedResult<InterviewDto>> GetAllAsync(PaginationRequest pagination, InterviewFilter? filter = null, CancellationToken ct = default)
    {
        var count = await _repository.CountAsync(filter, ct);
        var items = await _repository.GetAllDtoAsync(pagination, filter, ct);
        return items.ToPaginatedResult(pagination, count);
    }

    public async Task<InterviewDetailDto?> GetByIdAsync(int id, CancellationToken ct = default)
        => await _repository.GetDetailByIdAsync(id, ct);

    public async Task<InterviewDto?> CreateAsync(CreateInterviewRequest request, CancellationToken ct = default)
    {
        var entity = request.ToEntity();
        entity.CreatedAt = DateTime.UtcNow;

        // Auto-create Zoom meeting if requested
        if (request.CreateZoomMeeting)
        {
            Console.WriteLine($"[DEBUG] CreateZoomMeeting is TRUE - Creating Zoom meeting...");

            var applicant = entity.ApplicationId.HasValue
                ? await _appRepository.GetByIdAsync(entity.ApplicationId.Value, ct)
                : null;

            Console.WriteLine($"[DEBUG] Applicant: {applicant?.ApplicantName}");

            var zoomRequest = new ZoomMeetingRequest
            {
                Topic = $"Interview: {applicant?.ApplicantName ?? "Candidate"}",
                StartTime = request.ScheduledTime,
                DurationMinutes = request.DurationMinutes,
                TimeZone = "Asia/Ho_Chi_Minh",
                Agenda = $"Interview Type: {request.InterviewType}"
            };

            try
            {
                Console.WriteLine($"[DEBUG] Calling ZoomService...");
                var zoomMeeting = await _zoomService.CreateMeetingAsync(zoomRequest, ct);
                Console.WriteLine($"[DEBUG] Zoom meeting result: {zoomMeeting?.JoinUrl ?? "NULL"}");

                if (zoomMeeting != null)
                {
                    entity.MeetingLink = zoomMeeting.JoinUrl;
                    Console.WriteLine($"[DEBUG] Set MeetingLink to: {zoomMeeting.JoinUrl}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Zoom meeting creation failed: {ex}");
            }
        }
        else
        {
            Console.WriteLine($"[DEBUG] CreateZoomMeeting is FALSE or NULL - Skipping Zoom");
        }

        await _repository.AddAsync(entity, ct);

        // Send notification to applicant
        if (entity.ApplicationId.HasValue)
        {
            var app = await _appRepository.GetByIdAsync(entity.ApplicationId.Value, ct);
            if (app is not null)
            {
                var userId = await GetUserIdByEmailAsync(app.ApplicantEmail, ct);
                if (userId.HasValue)
                {
                    var interviewer = await _userRepository.GetByIdAsync(request.InterviewerId, ct);
                    await NotifyUserAsync(userId.Value,
                        $"Interview Scheduled: {entity.InterviewType}",
                        $"You have an interview scheduled on {entity.ScheduledTime:MMM d, yyyy 'at' h:mm tt} ({entity.InterviewType})." +
                        $" {(string.IsNullOrEmpty(entity.MeetingLink) ? "" : $"Meeting link: {entity.MeetingLink}")}",
                        "Interview", entity.InterviewId, "Interview", ct);
                }
            }
        }
        return entity.ToDto();
    }

    public async Task<InterviewDto?> UpdateAsync(int id, UpdateInterviewRequest request, CancellationToken ct = default)
    {
        var entity = await _repository.GetByIdAsync(id, ct);
        if (entity is null) return null;

        var wasCompleted = entity.Status == "Completed";
        var isNowCompleted = request.Status == "Completed";

        if (request.CampaignId.HasValue) entity.CampaignId = request.CampaignId;
        if (request.ApplicationId.HasValue) entity.ApplicationId = request.ApplicationId;
        if (request.InterviewerId.HasValue) entity.InterviewerId = request.InterviewerId.Value;
        if (request.ScheduledTime.HasValue) entity.ScheduledTime = request.ScheduledTime.Value;
        if (request.DurationMinutes.HasValue) entity.DurationMinutes = request.DurationMinutes.Value;
        if (request.InterviewType is not null) entity.InterviewType = request.InterviewType;
        if (request.MeetingLink is not null) entity.MeetingLink = request.MeetingLink;
        if (request.Location is not null) entity.Location = request.Location;
        if (request.Status is not null) entity.Status = request.Status;
        if (request.Feedback is not null) entity.Feedback = request.Feedback;
        if (request.Rating.HasValue) entity.Rating = request.Rating;
        entity.UpdatedAt = DateTime.UtcNow;

        // Auto-update Application + send notification when interview is marked Completed
        if (!wasCompleted && isNowCompleted && entity.ApplicationId.HasValue)
        {
            var app = await _appRepository.GetByIdAsync(entity.ApplicationId.Value, ct);
            if (app is not null)
            {
                var rating = request.Rating ?? entity.Rating;
                app.Status = rating.HasValue && rating >= 3 ? "Offered" : "Rejected";
                app.ReviewedBy = entity.InterviewerId;
                app.ReviewedDate = DateTime.UtcNow;
                await _appRepository.UpdateAsync(app, ct);

                var userId = await GetUserIdByEmailAsync(app.ApplicantEmail, ct);
                if (userId.HasValue)
                {
                    var interviewer = await _userRepository.GetByIdAsync(entity.InterviewerId, ct);
                    var resultStatus = app.Status;
                    var resultMsg = resultStatus == "Offered"
                        ? $"Congratulations! You have been {resultStatus}! You will receive further instructions soon."
                        : $"We regret to inform you that you were not selected for this position. Thank you for your interest.";
                    await NotifyUserAsync(userId.Value,
                        $"Interview Result: {resultStatus}",
                        $"Your interview has been completed. Result: {resultStatus}. {resultMsg}",
                        "Interview", entity.InterviewId, "Interview", ct);
                }
            }
        }

        await _repository.UpdateAsync(entity, ct);
        return entity.ToDto();
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        if (!await _repository.ExistsAsync(id, ct)) return false;
        await _repository.DeleteAsync(id, ct);
        return true;
    }
}
