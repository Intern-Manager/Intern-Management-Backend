using InternManagement.Application.DTOs;

namespace InternManagement.Application.Services;

public interface IRoleService
{
    Task<IEnumerable<RoleDto>> GetAllAsync(CancellationToken ct = default);
    Task<RoleDto?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<RoleDto> CreateAsync(CreateRoleRequest request, CancellationToken ct = default);
    Task<RoleDto?> UpdateAsync(int id, UpdateRoleRequest request, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
}

public interface IUserService
{
    Task<PaginatedResult<UserDto>> GetAllAsync(PaginationRequest pagination, UserFilter? filter = null, CancellationToken ct = default);
    Task<UserDetailDto?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<UserDetailDto?> GetByEmailAsync(string email, CancellationToken ct = default);
    Task<UserDto?> CreateAsync(CreateUserRequest request, CancellationToken ct = default);
    Task<UserDto?> UpdateAsync(int id, UpdateUserRequest request, CancellationToken ct = default);
    Task<bool> UpdateAvatarAsync(int id, string avatarUrl, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
    Task<IEnumerable<ChatContactDto>> GetChatContactsAsync(int currentUserId, CancellationToken ct = default);
}

public interface IInternProfileService
{
    Task<PaginatedResult<InternProfileDto>> GetAllAsync(PaginationRequest pagination, InternProfileFilter? filter = null, CancellationToken ct = default);
    Task<InternProfileDetailDto?> GetByIdAsync(int userId, CancellationToken ct = default);
    Task<InternProfileDto?> CreateAsync(CreateInternProfileRequest request, CancellationToken ct = default);
    Task<InternProfileDto?> UpdateAsync(int userId, UpdateInternProfileRequest request, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
}

public interface IInternshipCampaignService
{
    Task<PaginatedResult<InternshipCampaignDto>> GetAllAsync(PaginationRequest pagination, InternshipCampaignFilter? filter = null, CancellationToken ct = default);
    Task<InternshipCampaignDetailDto?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<InternshipCampaignDto?> CreateAsync(CreateInternshipCampaignRequest request, CancellationToken ct = default);
    Task<InternshipCampaignDto?> UpdateAsync(int id, UpdateInternshipCampaignRequest request, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
}

public interface ICampaignApplicationService
{
    Task<PaginatedResult<CampaignApplicationDto>> GetAllAsync(PaginationRequest pagination, CampaignApplicationFilter? filter = null, CancellationToken ct = default);
    Task<CampaignApplicationDetailDto?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<CampaignApplicationDto?> CreateAsync(CreateCampaignApplicationRequest request, CancellationToken ct = default);
    Task<CampaignApplicationDto?> UpdateAsync(int id, UpdateCampaignApplicationRequest request, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
}

public interface IInterviewService
{
    Task<PaginatedResult<InterviewDto>> GetAllAsync(PaginationRequest pagination, InterviewFilter? filter = null, CancellationToken ct = default);
    Task<InterviewDetailDto?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<InterviewDto?> CreateAsync(CreateInterviewRequest request, CancellationToken ct = default);
    Task<InterviewDto?> UpdateAsync(int id, UpdateInterviewRequest request, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
}

public interface ITrainingProgramService
{
    Task<PaginatedResult<TrainingProgramDto>> GetAllAsync(PaginationRequest pagination, TrainingProgramFilter? filter = null, CancellationToken ct = default);
    Task<TrainingProgramDetailDto?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<TrainingProgramDto?> CreateAsync(CreateTrainingProgramRequest request, CancellationToken ct = default);
    Task<TrainingProgramDto?> UpdateAsync(int id, UpdateTrainingProgramRequest request, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
}

public interface ILearningResourceService
{
    Task<PaginatedResult<LearningResourceDto>> GetAllAsync(PaginationRequest pagination, LearningResourceFilter? filter = null, CancellationToken ct = default);
    Task<LearningResourceDetailDto?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<LearningResourceDto?> CreateAsync(CreateLearningResourceRequest request, int uploadedBy, CancellationToken ct = default);
    Task<LearningResourceDto?> UpdateAsync(int id, UpdateLearningResourceRequest request, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
}

public interface IMentorshipService
{
    Task<PaginatedResult<MentorshipDto>> GetAllAsync(PaginationRequest pagination, MentorshipFilter? filter = null, CancellationToken ct = default);
    Task<MentorshipDetailDto?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<MentorshipDto?> CreateAsync(CreateMentorshipRequest request, CancellationToken ct = default);
    Task<MentorshipDto?> UpdateAsync(int id, UpdateMentorshipRequest request, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
}

public interface ITaskItemService
{
    Task<PaginatedResult<TaskItemDto>> GetAllAsync(PaginationRequest pagination, TaskItemFilter? filter = null, CancellationToken ct = default);
    Task<PaginatedResult<TaskItemDto>> GetByInternAsync(int internId, PaginationRequest pagination, CancellationToken ct = default);
    Task<PaginatedResult<TaskItemDto>> GetByMentorAsync(int mentorId, PaginationRequest pagination, CancellationToken ct = default);
    Task<TaskItemDetailDto?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<TaskItemDto?> CreateAsync(CreateTaskItemRequest request, CancellationToken ct = default);
    Task<TaskItemDto?> UpdateAsync(int id, UpdateTaskItemRequest request, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
}

public interface ITaskSubmissionService
{
    Task<IEnumerable<TaskSubmissionDto>> GetByTaskIdAsync(int taskId, CancellationToken ct = default);
    Task<TaskSubmissionDto?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<TaskSubmissionDto?> CreateAsync(CreateTaskSubmissionRequest request, CancellationToken ct = default);
    Task<TaskSubmissionDto?> GradeAsync(int id, int gradedBy, GradeSubmissionRequest request, CancellationToken ct = default);
}

public interface IDailyLogService
{
    Task<PaginatedResult<DailyLogDto>> GetAllAsync(PaginationRequest pagination, DailyLogFilter? filter = null, CancellationToken ct = default);
    Task<DailyLogDetailDto?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<DailyLogDto?> CreateAsync(CreateDailyLogRequest request, CancellationToken ct = default);
    Task<DailyLogDto?> UpdateAsync(int id, UpdateDailyLogRequest request, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
}

public interface IAssessmentService
{
    Task<PaginatedResult<AssessmentDto>> GetAllAsync(PaginationRequest pagination, AssessmentFilter? filter = null, CancellationToken ct = default);
    Task<AssessmentDetailDto?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<AssessmentDto?> CreateAsync(CreateAssessmentRequest request, CancellationToken ct = default);
    Task<AssessmentDto?> UpdateAsync(int id, UpdateAssessmentRequest request, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
}

public interface IFeedbackService
{
    Task<PaginatedResult<FeedbackDto>> GetAllAsync(PaginationRequest pagination, FeedbackFilter? filter = null, CancellationToken ct = default);
    Task<FeedbackDetailDto?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<FeedbackDto?> CreateAsync(CreateFeedbackRequest request, CancellationToken ct = default);
    Task<FeedbackDto?> UpdateAsync(int id, UpdateFeedbackRequest request, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
}

public interface ICommunicationService
{
    Task<PaginatedResult<CommunicationDto>> GetAllAsync(PaginationRequest pagination, CommunicationFilter? filter = null, CancellationToken ct = default);
    Task<CommunicationDetailDto?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<CommunicationDto?> CreateAsync(CreateCommunicationRequest request, CancellationToken ct = default);
    Task<CommunicationDto?> UpdateAsync(int id, UpdateCommunicationRequest request, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
    Task<IEnumerable<ChatContactDto>> GetConversationsAsync(int userId, CancellationToken ct = default);
    Task<PaginatedResult<CommunicationDto>> GetConversationMessagesAsync(int currentUserId, int otherUserId, PaginationRequest pagination, CancellationToken ct = default);
    Task<PaginatedResult<CommunicationDto>> GetMyMessagesAsync(int currentUserId, int? otherUserId, PaginationRequest pagination, CancellationToken ct = default);
    Task<int> GetUnreadCountAsync(int userId, CancellationToken ct = default);
    Task MarkAsReadAsync(int senderId, int receiverId, CancellationToken ct = default);
}

public interface INotificationService
{
    Task<PaginatedResult<NotificationDto>> GetAllAsync(PaginationRequest pagination, NotificationFilter? filter = null, CancellationToken ct = default);
    Task<NotificationDetailDto?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<NotificationDto?> CreateAsync(CreateNotificationRequest request, CancellationToken ct = default);
    Task<NotificationDto?> UpdateAsync(int id, UpdateNotificationRequest request, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
}

public interface IReportService
{
    Task<PaginatedResult<ReportDto>> GetAllAsync(PaginationRequest pagination, ReportFilter? filter = null, CancellationToken ct = default);
    Task<ReportDetailDto?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<ReportDto?> CreateAsync(CreateReportRequest request, CancellationToken ct = default);
    Task<ReportDto?> UpdateAsync(int id, UpdateReportRequest request, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
}

public interface IAttendanceService
{
    Task<PaginatedResult<AttendanceDto>> GetAllAsync(PaginationRequest pagination, AttendanceFilter? filter = null, CancellationToken ct = default);
    Task<AttendanceDetailDto?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<AttendanceDto?> CreateAsync(CreateAttendanceRequest request, CancellationToken ct = default);
    Task<AttendanceDto?> UpdateAsync(int id, UpdateAttendanceRequest request, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
}

public interface ICertificateService
{
    Task<PaginatedResult<CertificateDto>> GetAllAsync(PaginationRequest pagination, CertificateFilter? filter = null, CancellationToken ct = default);
    Task<CertificateDetailDto?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<CertificateDto?> CreateAsync(CreateCertificateRequest request, CancellationToken ct = default);
    Task<CertificateDto?> UpdateAsync(int id, UpdateCertificateRequest request, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
}

public interface IDepartmentService
{
    Task<PaginatedResult<DepartmentDto>> GetAllAsync(PaginationRequest pagination, DepartmentFilter? filter = null, CancellationToken ct = default);
    Task<DepartmentDetailDto?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<DepartmentDto?> CreateAsync(CreateDepartmentRequest request, CancellationToken ct = default);
    Task<DepartmentDto?> UpdateAsync(int id, UpdateDepartmentRequest request, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
}

public enum EmailTemplateType
{
    Welcome,
    PasswordReset,
    InterviewInvitation,
    InterviewReminder,
    ApplicationReceived,
    ApplicationStatusUpdate,
    TrainingEnrollment,
    AssessmentNotification,
    DailyLogReminder,
    NewTaskAssigned,
    TaskDeadlineReminder
}

public class EmailRequest
{
    public required string ToEmail { get; set; }
    public required string ToName { get; set; }
    public required string Subject { get; set; }
    public required string HtmlContent { get; set; }
    public EmailTemplateType TemplateType { get; set; }
    public Dictionary<string, string>? Placeholders { get; set; }
}

public class InterviewEmailData
{
    public required string CandidateName { get; set; }
    public string? CandidateEmail { get; set; }
    public required string Position { get; set; }
    public required DateTime InterviewDate { get; set; }
    public required string StartTime { get; set; }
    public required string Duration { get; set; }
    public required string InterviewType { get; set; }
    public string? MeetingLink { get; set; }
    public string? Location { get; set; }
    public required string InterviewerName { get; set; }
    public string? InterviewerEmail { get; set; }
}

public class ApplicationEmailData
{
    public required string ApplicantName { get; set; }
    public string? ApplicantEmail { get; set; }
    public required string CampaignTitle { get; set; }
    public required string Status { get; set; }
    public string? StatusMessage { get; set; }
    public string? NextSteps { get; set; }
}

public class TrainingEmailData
{
    public required string InternName { get; set; }
    public string? InternEmail { get; set; }
    public required string ProgramName { get; set; }
    public required DateTime StartDate { get; set; }
    public required DateTime EndDate { get; set; }
    public required string Schedule { get; set; }
    public string? Description { get; set; }
}

public class TaskEmailData
{
    public required string InternName { get; set; }
    public string? InternEmail { get; set; }
    public required string TaskTitle { get; set; }
    public required string DueDate { get; set; }
    public required string Priority { get; set; }
    public string? Description { get; set; }
    public required string AssignedByName { get; set; }
}

public interface IEmailService
{
    Task<bool> SendEmailAsync(EmailRequest request, CancellationToken ct = default);
    Task<bool> SendWelcomeEmailAsync(string email, string name, string tempPassword, CancellationToken ct = default);
    Task<bool> SendPasswordResetEmailAsync(string email, string name, string resetLink, CancellationToken ct = default);
    Task<bool> SendInterviewInvitationAsync(InterviewEmailData data, CancellationToken ct = default);
    Task<bool> SendInterviewReminderAsync(InterviewEmailData data, CancellationToken ct = default);
    Task<bool> SendApplicationReceivedEmailAsync(ApplicationEmailData data, CancellationToken ct = default);
    Task<bool> SendApplicationStatusUpdateEmailAsync(ApplicationEmailData data, CancellationToken ct = default);
    Task<bool> SendTrainingEnrollmentEmailAsync(TrainingEmailData data, CancellationToken ct = default);
    Task<bool> SendAssessmentNotificationAsync(string email, string internName, string assessmentType, DateTime dueDate, CancellationToken ct = default);
    Task<bool> SendDailyLogReminderAsync(string email, string internName, CancellationToken ct = default);
    Task<bool> SendNewTaskEmailAsync(TaskEmailData data, CancellationToken ct = default);
    Task<bool> SendTaskDeadlineReminderAsync(TaskEmailData data, CancellationToken ct = default);
}

public class CalendarEventRequest
{
    public required string Summary { get; set; }
    public string? Description { get; set; }
    public required DateTime StartTime { get; set; }
    public required DateTime EndTime { get; set; }
    public string? Location { get; set; }
    public List<string>? AttendeeEmails { get; set; }
    public bool SendNotifications { get; set; } = true;
    public List<int>? ReminderMinutes { get; set; }
    public string? TimeZone { get; set; } = "Asia/Ho_Chi_Minh";
}

public class CalendarEventResponse
{
    public required string EventId { get; set; }
    public required string HtmlLink { get; set; }
    public string? MeetingLink { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class UserCalendarConnection
{
    public required int UserId { get; set; }
    public required string GoogleRefreshToken { get; set; }
    public DateTime ConnectedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }
}

public interface ICalendarService
{
    Task<bool> IsUserConnectedAsync(int userId, CancellationToken ct = default);
    Task<string> GetAuthorizationUrlAsync(int userId, CancellationToken ct = default);
    Task<bool> HandleOAuthCallbackAsync(string code, int userId, CancellationToken ct = default);
    Task<bool> DisconnectCalendarAsync(int userId, CancellationToken ct = default);
    Task<CalendarEventResponse?> CreateEventAsync(int userId, CalendarEventRequest request, CancellationToken ct = default);
    Task<CalendarEventResponse?> CreateInterviewEventAsync(int createdByUserId, int interviewId, CancellationToken ct = default);
    Task<CalendarEventResponse?> CreateTrainingEventAsync(int createdByUserId, int programId, CancellationToken ct = default);
    Task<CalendarEventResponse?> CreateTaskReminderEventAsync(int internUserId, int taskId, CancellationToken ct = default);
    Task<bool> UpdateEventAsync(int userId, string eventId, CalendarEventRequest request, CancellationToken ct = default);
    Task<bool> DeleteEventAsync(int userId, string eventId, CancellationToken ct = default);
    Task<IEnumerable<CalendarEventResponse>> GetUpcomingEventsAsync(int userId, int maxResults = 10, CancellationToken ct = default);
}

public class ZoomMeetingRequest
{
    public required string Topic { get; set; }
    public required DateTime StartTime { get; set; }
    public int DurationMinutes { get; set; } = 60;
    public string? Agenda { get; set; }
    public List<string>? ParticipantEmails { get; set; }
    public bool AutoRecord { get; set; } = false;
    public string TimeZone { get; set; } = "Asia/Ho_Chi_Minh";
}

public class ZoomMeetingResponse
{
    public required string MeetingId { get; set; }
    public required string Topic { get; set; }
    public required string JoinUrl { get; set; }
    public required string StartUrl { get; set; }
    public string? Password { get; set; }
    public DateTime StartTime { get; set; }
    public int Duration { get; set; }
    public DateTime CreatedAt { get; set; }
}

public interface IZoomService
{
    Task<ZoomMeetingResponse?> CreateMeetingAsync(ZoomMeetingRequest request, CancellationToken ct = default);
    Task<ZoomMeetingResponse?> CreateInterviewMeetingAsync(int interviewId, CancellationToken ct = default);
    Task<ZoomMeetingResponse?> GetMeetingAsync(string meetingId, CancellationToken ct = default);
    Task<bool> DeleteMeetingAsync(string meetingId, CancellationToken ct = default);
    Task<string?> GetMeetingRecordingAsync(string meetingId, CancellationToken ct = default);
    Task<IEnumerable<ZoomMeetingResponse>> GetUpcomingMeetingsAsync(int maxResults = 10, CancellationToken ct = default);
}
