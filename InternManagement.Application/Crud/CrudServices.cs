using InternManagement.Application.DTOs;
using InternManagement.Application.Repositories;
using InternManagement.Application.Services;
using InternManagement.Domain.Entities;

namespace InternManagement.Application.Crud;

public class RoleService : IRoleService
{
    private readonly IRoleRepository _repository;

    public RoleService(IRoleRepository repository) => _repository = repository;

    public async Task<IEnumerable<RoleDto>> GetAllAsync(CancellationToken ct = default)
    {
        var roles = await _repository.GetAllAsync(ct);
        return roles.Select(r => r.ToDto());
    }

    public async Task<RoleDto?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var role = await _repository.GetByIdAsync(id, ct);
        return role?.ToDto();
    }

    public async Task<RoleDto> CreateAsync(CreateRoleRequest request, CancellationToken ct = default)
    {
        var role = request.ToEntity();
        await _repository.AddAsync(role, ct);
        return role.ToDto();
    }

    public async Task<RoleDto?> UpdateAsync(int id, UpdateRoleRequest request, CancellationToken ct = default)
    {
        var role = await _repository.GetByIdAsync(id, ct);
        if (role is null) return null;
        role.RoleName = request.RoleName ?? role.RoleName;
        role.Description = request.Description;
        await _repository.UpdateAsync(role, ct);
        return role.ToDto();
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        if (!await _repository.ExistsAsync(id, ct)) return false;
        await _repository.DeleteAsync(id, ct);
        return true;
    }
}

public class UserService : IUserService
{
    private readonly IUserRepository _repository;

    public UserService(IUserRepository repository) => _repository = repository;

    public async Task<PaginatedResult<UserDto>> GetAllAsync(PaginationRequest pagination, UserFilter? filter = null, CancellationToken ct = default)
    {
        var count = await _repository.CountAsync(filter, ct);
        var users = await _repository.GetAllDtoAsync(pagination, filter, ct);
        return users.Select(u => new UserDto(
            u.UserId, u.FullName, u.Email, u.Phone, u.AvatarUrl,
            u.RoleId, u.Status, u.EmailVerified, u.EmailVerifiedAt,
            u.LastLogin, u.CreatedAt, u.UpdatedAt))
            .ToPaginatedResult(pagination, count);
    }

    public async Task<UserDetailDto?> GetByIdAsync(int id, CancellationToken ct = default)
        => await _repository.GetDetailByIdAsync(id, ct);

    public async Task<UserDto?> CreateAsync(CreateUserRequest request, CancellationToken ct = default)
        => throw new NotImplementedException("Use AuthService for user registration");

    public async Task<UserDto?> UpdateAsync(int id, UpdateUserRequest request, CancellationToken ct = default)
    {
        var user = await _repository.GetByIdAsync(id, ct);
        if (user is null) return null;

        if (request.FullName is not null) user.FullName = request.FullName;
        if (request.Phone is not null) user.Phone = request.Phone;
        if (request.AvatarUrl is not null) user.AvatarUrl = request.AvatarUrl;
        if (request.RoleId.HasValue) user.RoleId = request.RoleId.Value;
        if (request.Status is not null) user.Status = request.Status;
        user.UpdatedAt = DateTime.UtcNow;

        await _repository.UpdateAsync(user, ct);
        return user.ToDto();
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        if (!await _repository.ExistsAsync(id, ct)) return false;
        await _repository.DeleteAsync(id, ct);
        return true;
    }
}

public class InternProfileService : IInternProfileService
{
    private readonly IInternProfileRepository _repository;

    public InternProfileService(IInternProfileRepository repository) => _repository = repository;

    public async Task<PaginatedResult<InternProfileDto>> GetAllAsync(PaginationRequest pagination, InternProfileFilter? filter = null, CancellationToken ct = default)
    {
        var count = await _repository.CountAsync(filter, ct);
        var items = await _repository.GetAllDtoAsync(pagination, filter, ct);
        return items.ToPaginatedResult(pagination, count);
    }

    public async Task<InternProfileDetailDto?> GetByIdAsync(int id, CancellationToken ct = default)
        => await _repository.GetDetailByIdAsync(id, ct);

    public async Task<InternProfileDto?> CreateAsync(int userId, CreateInternProfileRequest request, CancellationToken ct = default)
    {
        var entity = request.ToEntity();
        entity.CreatedAt = DateTime.UtcNow;
        await _repository.AddAsync(entity, ct);
        return entity.ToDto();
    }

    public async Task<InternProfileDto?> UpdateAsync(int id, UpdateInternProfileRequest request, CancellationToken ct = default)
    {
        var entity = await _repository.GetByIdAsync(id, ct);
        if (entity is null) return null;

        if (request.DateOfBirth.HasValue) entity.DateOfBirth = request.DateOfBirth;
        if (request.Address is not null) entity.Address = request.Address;
        if (request.University is not null) entity.University = request.University;
        if (request.Major is not null) entity.Major = request.Major;
        if (request.GraduationYear.HasValue) entity.GraduationYear = request.GraduationYear;
        if (request.EducationalBackground is not null) entity.EducationalBackground = request.EducationalBackground;
        if (request.WorkHistory is not null) entity.WorkHistory = request.WorkHistory;
        if (request.Skills is not null) entity.Skills = request.Skills;
        if (request.CvUrl is not null) entity.CvUrl = request.CvUrl;
        if (request.LinkedinUrl is not null) entity.LinkedinUrl = request.LinkedinUrl;
        if (request.GithubUrl is not null) entity.GithubUrl = request.GithubUrl;
        entity.UpdatedAt = DateTime.UtcNow;

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

public class InternshipCampaignService : IInternshipCampaignService
{
    private readonly IInternshipCampaignRepository _repository;

    public InternshipCampaignService(IInternshipCampaignRepository repository) => _repository = repository;

    public async Task<PaginatedResult<InternshipCampaignDto>> GetAllAsync(PaginationRequest pagination, InternshipCampaignFilter? filter = null, CancellationToken ct = default)
    {
        var count = await _repository.CountAsync(filter, ct);
        var items = await _repository.GetAllDtoAsync(pagination, filter, ct);
        return items.ToPaginatedResult(pagination, count);
    }

    public async Task<InternshipCampaignDetailDto?> GetByIdAsync(int id, CancellationToken ct = default)
        => await _repository.GetDetailByIdAsync(id, ct);

    public async Task<InternshipCampaignDto?> CreateAsync(CreateInternshipCampaignRequest request, CancellationToken ct = default)
    {
        var entity = request.ToEntity(0);
        entity.CreatedAt = DateTime.UtcNow;
        await _repository.AddAsync(entity, ct);
        return entity.ToDto();
    }

    public async Task<InternshipCampaignDto?> UpdateAsync(int id, UpdateInternshipCampaignRequest request, CancellationToken ct = default)
    {
        var entity = await _repository.GetByIdAsync(id, ct);
        if (entity is null) return null;

        if (request.Title is not null) entity.Title = request.Title;
        if (request.Description is not null) entity.Description = request.Description;
        if (request.Requirements is not null) entity.Requirements = request.Requirements;
        if (request.NumberOfPositions.HasValue) entity.NumberOfPositions = request.NumberOfPositions.Value;
        if (request.Department is not null) entity.Department = request.Department;
        if (request.Location is not null) entity.Location = request.Location;
        if (request.StartDate.HasValue) entity.StartDate = request.StartDate;
        if (request.EndDate.HasValue) entity.EndDate = request.EndDate;
        if (request.ApplicationDeadline.HasValue) entity.ApplicationDeadline = request.ApplicationDeadline;
        if (request.Status is not null) entity.Status = request.Status;
        entity.UpdatedAt = DateTime.UtcNow;

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

public class CampaignApplicationService : ICampaignApplicationService
{
    private readonly ICampaignApplicationRepository _repository;

    public CampaignApplicationService(ICampaignApplicationRepository repository) => _repository = repository;

    public async Task<PaginatedResult<CampaignApplicationDto>> GetAllAsync(PaginationRequest pagination, CampaignApplicationFilter? filter = null, CancellationToken ct = default)
    {
        var count = await _repository.CountAsync(filter, ct);
        var items = await _repository.GetAllDtoAsync(pagination, filter, ct);
        return items.ToPaginatedResult(pagination, count);
    }

    public async Task<CampaignApplicationDetailDto?> GetByIdAsync(int id, CancellationToken ct = default)
        => await _repository.GetDetailByIdAsync(id, ct);

    public async Task<CampaignApplicationDto?> CreateAsync(CreateCampaignApplicationRequest request, CancellationToken ct = default)
    {
        var entity = request.ToEntity();
        entity.AppliedDate = DateTime.UtcNow;
        await _repository.AddAsync(entity, ct);
        return entity.ToDto();
    }

    public async Task<CampaignApplicationDto?> UpdateAsync(int id, UpdateCampaignApplicationRequest request, CancellationToken ct = default)
    {
        var entity = await _repository.GetByIdAsync(id, ct);
        if (entity is null) return null;

        if (request.Status is not null) entity.Status = request.Status;
        if (request.ReviewedBy.HasValue) entity.ReviewedBy = request.ReviewedBy;
        if (request.Notes is not null) entity.Notes = request.Notes;
        if (entity.Status != "Pending") entity.ReviewedDate = DateTime.UtcNow;

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

public class InterviewService : IInterviewService
{
    private readonly IInterviewRepository _repository;

    public InterviewService(IInterviewRepository repository) => _repository = repository;

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
        await _repository.AddAsync(entity, ct);
        return entity.ToDto();
    }

    public async Task<InterviewDto?> UpdateAsync(int id, UpdateInterviewRequest request, CancellationToken ct = default)
    {
        var entity = await _repository.GetByIdAsync(id, ct);
        if (entity is null) return null;

        if (request.ScheduledTime.HasValue) entity.ScheduledTime = request.ScheduledTime.Value;
        if (request.DurationMinutes.HasValue) entity.DurationMinutes = request.DurationMinutes.Value;
        if (request.InterviewType is not null) entity.InterviewType = request.InterviewType;
        if (request.MeetingLink is not null) entity.MeetingLink = request.MeetingLink;
        if (request.Location is not null) entity.Location = request.Location;
        if (request.Status is not null) entity.Status = request.Status;
        if (request.Feedback is not null) entity.Feedback = request.Feedback;
        if (request.Rating.HasValue) entity.Rating = request.Rating;
        entity.UpdatedAt = DateTime.UtcNow;

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

public class TrainingProgramService : ITrainingProgramService
{
    private readonly ITrainingProgramRepository _repository;

    public TrainingProgramService(ITrainingProgramRepository repository) => _repository = repository;

    public async Task<PaginatedResult<TrainingProgramDto>> GetAllAsync(PaginationRequest pagination, TrainingProgramFilter? filter = null, CancellationToken ct = default)
    {
        var count = await _repository.CountAsync(filter, ct);
        var items = await _repository.GetAllDtoAsync(pagination, filter, ct);
        return items.ToPaginatedResult(pagination, count);
    }

    public async Task<TrainingProgramDetailDto?> GetByIdAsync(int id, CancellationToken ct = default)
        => await _repository.GetDetailByIdAsync(id, ct);

    public async Task<TrainingProgramDto?> CreateAsync(CreateTrainingProgramRequest request, CancellationToken ct = default)
    {
        var entity = request.ToEntity();
        entity.CreatedAt = DateTime.UtcNow;
        await _repository.AddAsync(entity, ct);
        return entity.ToDto();
    }

    public async Task<TrainingProgramDto?> UpdateAsync(int id, UpdateTrainingProgramRequest request, CancellationToken ct = default)
    {
        var entity = await _repository.GetByIdAsync(id, ct);
        if (entity is null) return null;

        if (request.ProgramName is not null) entity.ProgramName = request.ProgramName;
        if (request.Description is not null) entity.Description = request.Description;
        if (request.Objectives is not null) entity.Objectives = request.Objectives;
        if (request.DurationWeeks.HasValue) entity.DurationWeeks = request.DurationWeeks;
        if (request.StartDate.HasValue) entity.StartDate = request.StartDate;
        if (request.EndDate.HasValue) entity.EndDate = request.EndDate;
        if (request.Status is not null) entity.Status = request.Status;
        entity.UpdatedAt = DateTime.UtcNow;

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

public class LearningResourceService : ILearningResourceService
{
    private readonly ILearningResourceRepository _repository;

    public LearningResourceService(ILearningResourceRepository repository) => _repository = repository;

    public async Task<PaginatedResult<LearningResourceDto>> GetAllAsync(PaginationRequest pagination, LearningResourceFilter? filter = null, CancellationToken ct = default)
    {
        var count = await _repository.CountAsync(filter, ct);
        var items = await _repository.GetAllDtoAsync(pagination, filter, ct);
        return items.ToPaginatedResult(pagination, count);
    }

    public async Task<LearningResourceDetailDto?> GetByIdAsync(int id, CancellationToken ct = default)
        => await _repository.GetDetailByIdAsync(id, ct);

    public async Task<LearningResourceDto?> CreateAsync(CreateLearningResourceRequest request, CancellationToken ct = default)
        => throw new NotImplementedException("CreateAsync requires uploadedBy parameter");

    public async Task<LearningResourceDto?> UpdateAsync(int id, UpdateLearningResourceRequest request, CancellationToken ct = default)
    {
        var entity = await _repository.GetByIdAsync(id, ct);
        if (entity is null) return null;

        if (request.Title is not null) entity.Title = request.Title;
        if (request.Description is not null) entity.Description = request.Description;
        if (request.ResourceUrl is not null) entity.ResourceUrl = request.ResourceUrl;
        if (request.ResourceType is not null) entity.ResourceType = request.ResourceType;
        if (request.FileSizeMb.HasValue) entity.FileSizeMb = request.FileSizeMb;

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

public class MentorshipService : IMentorshipService
{
    private readonly IMentorshipRepository _repository;

    public MentorshipService(IMentorshipRepository repository) => _repository = repository;

    public async Task<PaginatedResult<MentorshipDto>> GetAllAsync(PaginationRequest pagination, MentorshipFilter? filter = null, CancellationToken ct = default)
    {
        var count = await _repository.CountAsync(filter, ct);
        var items = await _repository.GetAllDtoAsync(pagination, filter, ct);
        return items.ToPaginatedResult(pagination, count);
    }

    public async Task<MentorshipDetailDto?> GetByIdAsync(int id, CancellationToken ct = default)
        => await _repository.GetDetailByIdAsync(id, ct);

    public async Task<MentorshipDto?> CreateAsync(CreateMentorshipRequest request, CancellationToken ct = default)
    {
        var entity = request.ToEntity();
        entity.CreatedAt = DateTime.UtcNow;
        await _repository.AddAsync(entity, ct);
        return entity.ToDto();
    }

    public async Task<MentorshipDto?> UpdateAsync(int id, UpdateMentorshipRequest request, CancellationToken ct = default)
    {
        var entity = await _repository.GetByIdAsync(id, ct);
        if (entity is null) return null;

        if (request.MentorId.HasValue) entity.MentorId = request.MentorId.Value;
        if (request.InternId.HasValue) entity.InternId = request.InternId.Value;
        if (request.ProgramId.HasValue) entity.ProgramId = request.ProgramId.Value;
        if (request.StartDate.HasValue) entity.StartDate = request.StartDate.Value;
        if (request.EndDate.HasValue) entity.EndDate = request.EndDate;
        if (request.Status is not null) entity.Status = request.Status;

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

public class TaskItemService : ITaskItemService
{
    private readonly ITaskItemRepository _repository;

    public TaskItemService(ITaskItemRepository repository) => _repository = repository;

    public async Task<PaginatedResult<TaskItemDto>> GetAllAsync(PaginationRequest pagination, TaskItemFilter? filter = null, CancellationToken ct = default)
    {
        var count = await _repository.CountAsync(filter, ct);
        var items = await _repository.GetAllDtoAsync(pagination, filter, ct);
        return items.ToPaginatedResult(pagination, count);
    }

    public async Task<TaskItemDetailDto?> GetByIdAsync(int id, CancellationToken ct = default)
        => await _repository.GetDetailByIdAsync(id, ct);

    public async Task<TaskItemDto?> CreateAsync(CreateTaskItemRequest request, CancellationToken ct = default)
    {
        var entity = request.ToEntity();
        entity.CreatedAt = DateTime.UtcNow;
        await _repository.AddAsync(entity, ct);
        return entity.ToDto();
    }

    public async Task<TaskItemDto?> UpdateAsync(int id, UpdateTaskItemRequest request, CancellationToken ct = default)
    {
        var entity = await _repository.GetByIdAsync(id, ct);
        if (entity is null) return null;

        if (request.Title is not null) entity.Title = request.Title;
        if (request.Description is not null) entity.Description = request.Description;
        if (request.DueDate.HasValue) entity.DueDate = request.DueDate;
        if (request.Priority is not null) entity.Priority = request.Priority;
        if (request.Status is not null) entity.Status = request.Status;
        if (request.CompletionDate.HasValue) entity.CompletionDate = request.CompletionDate;
        entity.UpdatedAt = DateTime.UtcNow;

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

public class DailyLogService : IDailyLogService
{
    private readonly IDailyLogRepository _repository;

    public DailyLogService(IDailyLogRepository repository) => _repository = repository;

    public async Task<PaginatedResult<DailyLogDto>> GetAllAsync(PaginationRequest pagination, DailyLogFilter? filter = null, CancellationToken ct = default)
    {
        var count = await _repository.CountAsync(filter, ct);
        var items = await _repository.GetAllDtoAsync(pagination, filter, ct);
        return items.ToPaginatedResult(pagination, count);
    }

    public async Task<DailyLogDetailDto?> GetByIdAsync(int id, CancellationToken ct = default)
        => await _repository.GetDetailByIdAsync(id, ct);

    public async Task<DailyLogDto?> CreateAsync(CreateDailyLogRequest request, CancellationToken ct = default)
    {
        var entity = request.ToEntity();
        entity.CreatedAt = DateTime.UtcNow;
        await _repository.AddAsync(entity, ct);
        return entity.ToDto();
    }

    public async Task<DailyLogDto?> UpdateAsync(int id, UpdateDailyLogRequest request, CancellationToken ct = default)
    {
        var entity = await _repository.GetByIdAsync(id, ct);
        if (entity is null) return null;

        if (request.LogDate.HasValue) entity.LogDate = request.LogDate.Value;
        if (request.ActivityDescription is not null) entity.ActivityDescription = request.ActivityDescription;
        if (request.HoursWorked.HasValue) entity.HoursWorked = request.HoursWorked;
        if (request.ChallengesFaced is not null) entity.ChallengesFaced = request.ChallengesFaced;
        if (request.MentorFeedback is not null) entity.MentorFeedback = request.MentorFeedback;
        if (request.KpiScore.HasValue) entity.KpiScore = request.KpiScore;
        entity.UpdatedAt = DateTime.UtcNow;

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

public class AssessmentService : IAssessmentService
{
    private readonly IAssessmentRepository _repository;

    public AssessmentService(IAssessmentRepository repository) => _repository = repository;

    public async Task<PaginatedResult<AssessmentDto>> GetAllAsync(PaginationRequest pagination, AssessmentFilter? filter = null, CancellationToken ct = default)
    {
        var count = await _repository.CountAsync(filter, ct);
        var items = await _repository.GetAllDtoAsync(pagination, filter, ct);
        return items.ToPaginatedResult(pagination, count);
    }

    public async Task<AssessmentDetailDto?> GetByIdAsync(int id, CancellationToken ct = default)
        => await _repository.GetDetailByIdAsync(id, ct);

    public async Task<AssessmentDto?> CreateAsync(CreateAssessmentRequest request, CancellationToken ct = default)
    {
        var entity = request.ToEntity();
        entity.CreatedAt = DateTime.UtcNow;
        await _repository.AddAsync(entity, ct);
        return entity.ToDto();
    }

    public async Task<AssessmentDto?> UpdateAsync(int id, UpdateAssessmentRequest request, CancellationToken ct = default)
    {
        var entity = await _repository.GetByIdAsync(id, ct);
        if (entity is null) return null;

        if (request.AssessmentDate.HasValue) entity.AssessmentDate = request.AssessmentDate.Value;
        if (request.AssessmentType is not null) entity.AssessmentType = request.AssessmentType;
        if (request.TechnicalSkillsScore.HasValue) entity.TechnicalSkillsScore = request.TechnicalSkillsScore;
        if (request.SoftSkillsScore.HasValue) entity.SoftSkillsScore = request.SoftSkillsScore;
        if (request.CommunicationScore.HasValue) entity.CommunicationScore = request.CommunicationScore;
        if (request.TeamworkScore.HasValue) entity.TeamworkScore = request.TeamworkScore;
        if (request.OverallRating.HasValue) entity.OverallRating = request.OverallRating;
        if (request.Strengths is not null) entity.Strengths = request.Strengths;
        if (request.AreasForImprovement is not null) entity.AreasForImprovement = request.AreasForImprovement;
        if (request.Comments is not null) entity.Comments = request.Comments;

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

public class FeedbackService : IFeedbackService
{
    private readonly IFeedbackRepository _repository;

    public FeedbackService(IFeedbackRepository repository) => _repository = repository;

    public async Task<PaginatedResult<FeedbackDto>> GetAllAsync(PaginationRequest pagination, FeedbackFilter? filter = null, CancellationToken ct = default)
    {
        var count = await _repository.CountAsync(filter, ct);
        var items = await _repository.GetAllDtoAsync(pagination, filter, ct);
        return items.ToPaginatedResult(pagination, count);
    }

    public async Task<FeedbackDetailDto?> GetByIdAsync(int id, CancellationToken ct = default)
        => await _repository.GetDetailByIdAsync(id, ct);

    public async Task<FeedbackDto?> CreateAsync(CreateFeedbackRequest request, CancellationToken ct = default)
    {
        var entity = request.ToEntity();
        entity.SubmittedAt = DateTime.UtcNow;
        await _repository.AddAsync(entity, ct);
        return entity.ToDto();
    }

    public async Task<FeedbackDto?> UpdateAsync(int id, UpdateFeedbackRequest request, CancellationToken ct = default)
    {
        var entity = await _repository.GetByIdAsync(id, ct);
        if (entity is null) return null;

        if (request.Rating.HasValue) entity.Rating = request.Rating;
        if (request.Comments is not null) entity.Comments = request.Comments;

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

public class CommunicationService : ICommunicationService
{
    private readonly ICommunicationRepository _repository;

    public CommunicationService(ICommunicationRepository repository) => _repository = repository;

    public async Task<PaginatedResult<CommunicationDto>> GetAllAsync(PaginationRequest pagination, CommunicationFilter? filter = null, CancellationToken ct = default)
    {
        var count = await _repository.CountAsync(filter, ct);
        var items = await _repository.GetAllDtoAsync(pagination, filter, ct);
        return items.ToPaginatedResult(pagination, count);
    }

    public async Task<CommunicationDetailDto?> GetByIdAsync(int id, CancellationToken ct = default)
        => await _repository.GetDetailByIdAsync(id, ct);

    public async Task<CommunicationDto?> CreateAsync(CreateCommunicationRequest request, CancellationToken ct = default)
    {
        var entity = request.ToEntity();
        entity.SentAt = DateTime.UtcNow;
        await _repository.AddAsync(entity, ct);
        return entity.ToDto();
    }

    public async Task<CommunicationDto?> UpdateAsync(int id, UpdateCommunicationRequest request, CancellationToken ct = default)
    {
        var entity = await _repository.GetByIdAsync(id, ct);
        if (entity is null) return null;

        if (request.IsRead.HasValue) entity.IsRead = request.IsRead.Value;
        if (request.ReadAt.HasValue) entity.ReadAt = request.ReadAt;

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

public class NotificationService : INotificationService
{
    private readonly INotificationRepository _repository;

    public NotificationService(INotificationRepository repository) => _repository = repository;

    public async Task<PaginatedResult<NotificationDto>> GetAllAsync(PaginationRequest pagination, NotificationFilter? filter = null, CancellationToken ct = default)
    {
        var count = await _repository.CountAsync(filter, ct);
        var items = await _repository.GetAllDtoAsync(pagination, filter, ct);
        return items.ToPaginatedResult(pagination, count);
    }

    public async Task<NotificationDetailDto?> GetByIdAsync(int id, CancellationToken ct = default)
        => await _repository.GetDetailByIdAsync(id, ct);

    public async Task<NotificationDto?> CreateAsync(CreateNotificationRequest request, CancellationToken ct = default)
    {
        var entity = request.ToEntity();
        entity.SentAt = DateTime.UtcNow;
        await _repository.AddAsync(entity, ct);
        return entity.ToDto();
    }

    public async Task<NotificationDto?> UpdateAsync(int id, UpdateNotificationRequest request, CancellationToken ct = default)
    {
        var entity = await _repository.GetByIdAsync(id, ct);
        if (entity is null) return null;

        if (request.IsRead.HasValue) entity.IsRead = request.IsRead.Value;
        if (request.ReadAt.HasValue) entity.ReadAt = request.ReadAt;

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

public class ReportService : IReportService
{
    private readonly IReportRepository _repository;

    public ReportService(IReportRepository repository) => _repository = repository;

    public async Task<PaginatedResult<ReportDto>> GetAllAsync(PaginationRequest pagination, ReportFilter? filter = null, CancellationToken ct = default)
    {
        var count = await _repository.CountAsync(filter, ct);
        var items = await _repository.GetAllDtoAsync(pagination, filter, ct);
        return items.ToPaginatedResult(pagination, count);
    }

    public async Task<ReportDetailDto?> GetByIdAsync(int id, CancellationToken ct = default)
        => await _repository.GetDetailByIdAsync(id, ct);

    public async Task<ReportDto?> CreateAsync(CreateReportRequest request, CancellationToken ct = default)
    {
        var entity = request.ToEntity();
        entity.GeneratedAt = DateTime.UtcNow;
        await _repository.AddAsync(entity, ct);
        return entity.ToDto();
    }

    public async Task<ReportDto?> UpdateAsync(int id, UpdateReportRequest request, CancellationToken ct = default)
    {
        var entity = await _repository.GetByIdAsync(id, ct);
        if (entity is null) return null;

        if (request.ReportName is not null) entity.ReportName = request.ReportName;
        if (request.ReportType is not null) entity.ReportType = request.ReportType;
        if (request.Description is not null) entity.Description = request.Description;
        if (request.FileUrl is not null) entity.FileUrl = request.FileUrl;
        if (request.FileFormat is not null) entity.FileFormat = request.FileFormat;
        if (request.Parameters is not null) entity.Parameters = request.Parameters;

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

public class AttendanceService : IAttendanceService
{
    private readonly IAttendanceRepository _repository;

    public AttendanceService(IAttendanceRepository repository) => _repository = repository;

    public async Task<PaginatedResult<AttendanceDto>> GetAllAsync(PaginationRequest pagination, AttendanceFilter? filter = null, CancellationToken ct = default)
    {
        var count = await _repository.CountAsync(filter, ct);
        var items = await _repository.GetAllDtoAsync(pagination, filter, ct);
        return items.ToPaginatedResult(pagination, count);
    }

    public async Task<AttendanceDetailDto?> GetByIdAsync(int id, CancellationToken ct = default)
        => await _repository.GetDetailByIdAsync(id, ct);

    public async Task<AttendanceDto?> CreateAsync(CreateAttendanceRequest request, CancellationToken ct = default)
    {
        var entity = request.ToEntity();
        entity.CreatedAt = DateTime.UtcNow;
        await _repository.AddAsync(entity, ct);
        return entity.ToDto();
    }

    public async Task<AttendanceDto?> UpdateAsync(int id, UpdateAttendanceRequest request, CancellationToken ct = default)
    {
        var entity = await _repository.GetByIdAsync(id, ct);
        if (entity is null) return null;

        if (request.CheckInTime.HasValue) entity.CheckInTime = request.CheckInTime;
        if (request.CheckOutTime.HasValue) entity.CheckOutTime = request.CheckOutTime;
        if (request.Status is not null) entity.Status = request.Status;
        if (request.Notes is not null) entity.Notes = request.Notes;
        if (request.ApprovedBy.HasValue) entity.ApprovedBy = request.ApprovedBy;

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

public class CertificateService : ICertificateService
{
    private readonly ICertificateRepository _repository;

    public CertificateService(ICertificateRepository repository) => _repository = repository;

    public async Task<PaginatedResult<CertificateDto>> GetAllAsync(PaginationRequest pagination, CertificateFilter? filter = null, CancellationToken ct = default)
    {
        var count = await _repository.CountAsync(filter, ct);
        var items = await _repository.GetAllDtoAsync(pagination, filter, ct);
        return items.ToPaginatedResult(pagination, count);
    }

    public async Task<CertificateDetailDto?> GetByIdAsync(int id, CancellationToken ct = default)
        => await _repository.GetDetailByIdAsync(id, ct);

    public async Task<CertificateDto?> CreateAsync(CreateCertificateRequest request, CancellationToken ct = default)
    {
        var entity = request.ToEntity();
        entity.CreatedAt = DateTime.UtcNow;
        await _repository.AddAsync(entity, ct);
        return entity.ToDto();
    }

    public async Task<CertificateDto?> UpdateAsync(int id, UpdateCertificateRequest request, CancellationToken ct = default)
    {
        var entity = await _repository.GetByIdAsync(id, ct);
        if (entity is null) return null;

        if (request.CertificateName is not null) entity.CertificateName = request.CertificateName;
        if (request.Description is not null) entity.Description = request.Description;
        if (request.IssuedDate.HasValue) entity.IssuedDate = request.IssuedDate.Value;
        if (request.CertificateUrl is not null) entity.CertificateUrl = request.CertificateUrl;

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
