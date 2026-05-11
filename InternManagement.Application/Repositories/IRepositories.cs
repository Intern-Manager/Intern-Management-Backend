using InternManagement.Domain.Entities;
using InternManagement.Application.DTOs;
using InternManagement.Application.Repositories;

namespace InternManagement.Application.Repositories;

public interface IRoleRepository : IGenericRepository<Role>
{
    Task<Role?> GetByNameAsync(string roleName, CancellationToken ct = default);
    Task<IEnumerable<RoleDto>> GetAllDtoAsync(CancellationToken ct = default);
}

public interface IUserRepository : IGenericRepository<User>
{
    Task<User?> FindByEmailAsync(string email, CancellationToken ct = default);
    Task<UserDetailDto?> GetDetailByIdAsync(int id, CancellationToken ct = default);
    Task<IEnumerable<UserDto>> GetAllDtoAsync(PaginationRequest pagination, UserFilter? filter = null, CancellationToken ct = default);
    Task<int> CountAsync(UserFilter? filter = null, CancellationToken ct = default);
    new Task UpdateAsync(User entity, CancellationToken ct = default);
}

public interface IInternProfileRepository : IGenericRepository<InternProfile>
{
    Task<InternProfileDetailDto?> GetDetailByIdAsync(int id, CancellationToken ct = default);
    Task<IEnumerable<InternProfileDto>> GetAllDtoAsync(PaginationRequest pagination, InternProfileFilter? filter = null, CancellationToken ct = default);
    Task<int> CountAsync(InternProfileFilter? filter = null, CancellationToken ct = default);
}

public interface IInternshipCampaignRepository : IGenericRepository<InternshipCampaign>
{
    Task<InternshipCampaignDetailDto?> GetDetailByIdAsync(int id, CancellationToken ct = default);
    Task<IEnumerable<InternshipCampaignDto>> GetAllDtoAsync(PaginationRequest pagination, InternshipCampaignFilter? filter = null, CancellationToken ct = default);
    Task<int> CountAsync(InternshipCampaignFilter? filter = null, CancellationToken ct = default);
}

public interface ICampaignApplicationRepository : IGenericRepository<CampaignApplication>
{
    Task<CampaignApplicationDetailDto?> GetDetailByIdAsync(int id, CancellationToken ct = default);
    Task<IEnumerable<CampaignApplicationDto>> GetAllDtoAsync(PaginationRequest pagination, CampaignApplicationFilter? filter = null, CancellationToken ct = default);
    Task<int> CountAsync(CampaignApplicationFilter? filter = null, CancellationToken ct = default);
}

public interface IInterviewRepository : IGenericRepository<Interview>
{
    Task<InterviewDetailDto?> GetDetailByIdAsync(int id, CancellationToken ct = default);
    Task<IEnumerable<InterviewDto>> GetAllDtoAsync(PaginationRequest pagination, InterviewFilter? filter = null, CancellationToken ct = default);
    Task<int> CountAsync(InterviewFilter? filter = null, CancellationToken ct = default);
}

public interface ITrainingProgramRepository : IGenericRepository<TrainingProgram>
{
    Task<TrainingProgramDetailDto?> GetDetailByIdAsync(int id, CancellationToken ct = default);
    Task<IEnumerable<TrainingProgramDto>> GetAllDtoAsync(PaginationRequest pagination, TrainingProgramFilter? filter = null, CancellationToken ct = default);
    Task<int> CountAsync(TrainingProgramFilter? filter = null, CancellationToken ct = default);
}

public interface ILearningResourceRepository : IGenericRepository<LearningResource>
{
    Task<LearningResourceDetailDto?> GetDetailByIdAsync(int id, CancellationToken ct = default);
    Task<IEnumerable<LearningResourceDto>> GetAllDtoAsync(PaginationRequest pagination, LearningResourceFilter? filter = null, CancellationToken ct = default);
    Task<int> CountAsync(LearningResourceFilter? filter = null, CancellationToken ct = default);
}

public interface IMentorshipRepository : IGenericRepository<Mentorship>
{
    Task<MentorshipDetailDto?> GetDetailByIdAsync(int id, CancellationToken ct = default);
    Task<IEnumerable<MentorshipDto>> GetAllDtoAsync(PaginationRequest pagination, MentorshipFilter? filter = null, CancellationToken ct = default);
    Task<int> CountAsync(MentorshipFilter? filter = null, CancellationToken ct = default);
}

public interface ITaskItemRepository : IGenericRepository<TaskItem>
{
    Task<TaskItemDetailDto?> GetDetailByIdAsync(int id, CancellationToken ct = default);
    Task<IEnumerable<TaskItemDto>> GetAllDtoAsync(PaginationRequest pagination, TaskItemFilter? filter = null, CancellationToken ct = default);
    Task<int> CountAsync(TaskItemFilter? filter = null, CancellationToken ct = default);
}

public interface IDailyLogRepository : IGenericRepository<DailyLog>
{
    Task<DailyLogDetailDto?> GetDetailByIdAsync(int id, CancellationToken ct = default);
    Task<IEnumerable<DailyLogDto>> GetAllDtoAsync(PaginationRequest pagination, DailyLogFilter? filter = null, CancellationToken ct = default);
    Task<int> CountAsync(DailyLogFilter? filter = null, CancellationToken ct = default);
}

public interface IAssessmentRepository : IGenericRepository<Assessment>
{
    Task<AssessmentDetailDto?> GetDetailByIdAsync(int id, CancellationToken ct = default);
    Task<IEnumerable<AssessmentDto>> GetAllDtoAsync(PaginationRequest pagination, AssessmentFilter? filter = null, CancellationToken ct = default);
    Task<int> CountAsync(AssessmentFilter? filter = null, CancellationToken ct = default);
}

public interface IFeedbackRepository : IGenericRepository<Feedback>
{
    Task<FeedbackDetailDto?> GetDetailByIdAsync(int id, CancellationToken ct = default);
    Task<IEnumerable<FeedbackDto>> GetAllDtoAsync(PaginationRequest pagination, FeedbackFilter? filter = null, CancellationToken ct = default);
    Task<int> CountAsync(FeedbackFilter? filter = null, CancellationToken ct = default);
}

public interface ICommunicationRepository : IGenericRepository<Communication>
{
    Task<CommunicationDetailDto?> GetDetailByIdAsync(int id, CancellationToken ct = default);
    Task<IEnumerable<CommunicationDto>> GetAllDtoAsync(PaginationRequest pagination, CommunicationFilter? filter = null, CancellationToken ct = default);
    Task<int> CountAsync(CommunicationFilter? filter = null, CancellationToken ct = default);
}

public interface INotificationRepository : IGenericRepository<Notification>
{
    Task<NotificationDetailDto?> GetDetailByIdAsync(int id, CancellationToken ct = default);
    Task<IEnumerable<NotificationDto>> GetAllDtoAsync(PaginationRequest pagination, NotificationFilter? filter = null, CancellationToken ct = default);
    Task<int> CountAsync(NotificationFilter? filter = null, CancellationToken ct = default);
}

public interface IReportRepository : IGenericRepository<Report>
{
    Task<ReportDetailDto?> GetDetailByIdAsync(int id, CancellationToken ct = default);
    Task<IEnumerable<ReportDto>> GetAllDtoAsync(PaginationRequest pagination, ReportFilter? filter = null, CancellationToken ct = default);
    Task<int> CountAsync(ReportFilter? filter = null, CancellationToken ct = default);
}

public interface IAttendanceRepository : IGenericRepository<Attendance>
{
    Task<AttendanceDetailDto?> GetDetailByIdAsync(int id, CancellationToken ct = default);
    Task<IEnumerable<AttendanceDto>> GetAllDtoAsync(PaginationRequest pagination, AttendanceFilter? filter = null, CancellationToken ct = default);
    Task<int> CountAsync(AttendanceFilter? filter = null, CancellationToken ct = default);
}

public interface ICertificateRepository : IGenericRepository<Certificate>
{
    Task<CertificateDetailDto?> GetDetailByIdAsync(int id, CancellationToken ct = default);
    Task<IEnumerable<CertificateDto>> GetAllDtoAsync(PaginationRequest pagination, CertificateFilter? filter = null, CancellationToken ct = default);
    Task<int> CountAsync(CertificateFilter? filter = null, CancellationToken ct = default);
}
