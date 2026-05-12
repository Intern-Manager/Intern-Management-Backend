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
    Task<UserDto?> CreateAsync(CreateUserRequest request, CancellationToken ct = default);
    Task<UserDto?> UpdateAsync(int id, UpdateUserRequest request, CancellationToken ct = default);
    Task<bool> UpdateAvatarAsync(int id, string avatarUrl, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
}

public interface IInternProfileService
{
    Task<PaginatedResult<InternProfileDto>> GetAllAsync(PaginationRequest pagination, InternProfileFilter? filter = null, CancellationToken ct = default);
    Task<InternProfileDetailDto?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<InternProfileDto?> CreateAsync(int userId, CreateInternProfileRequest request, CancellationToken ct = default);
    Task<InternProfileDto?> UpdateAsync(int id, UpdateInternProfileRequest request, CancellationToken ct = default);
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
    Task<LearningResourceDto?> CreateAsync(CreateLearningResourceRequest request, CancellationToken ct = default);
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
    Task<TaskItemDetailDto?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<TaskItemDto?> CreateAsync(CreateTaskItemRequest request, CancellationToken ct = default);
    Task<TaskItemDto?> UpdateAsync(int id, UpdateTaskItemRequest request, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
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
