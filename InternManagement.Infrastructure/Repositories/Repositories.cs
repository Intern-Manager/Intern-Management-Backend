using InternManagement.Application.DTOs;
using InternManagement.Application.Repositories;
using InternManagement.Domain.Entities;
using InternManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace InternManagement.Infrastructure.Repositories;

// Note: User & Role repositories are in Infrastructure.Auth namespace
// They implement both Auth and CRUD repository interfaces

public class EfInternProfileRepository : GenericRepository<InternProfile>, IInternProfileRepository
{
    public EfInternProfileRepository(AppDbContext context) : base(context) { }

    public async Task<InternProfileDetailDto?> GetDetailByIdAsync(int id, CancellationToken ct = default)
    {
        var profile = await _dbSet.FirstOrDefaultAsync(p => p.UserId == id, ct);
        if (profile is null) return null;

        var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == id, ct);

        return new InternProfileDetailDto(
            profile.InternId,
            profile.UserId,
            user?.FullName ?? "",
            user?.Email ?? "",
            user?.Phone,
            profile.DateOfBirth,
            profile.Address,
            profile.University,
            profile.Major,
            profile.GraduationYear,
            profile.EducationalBackground,
            profile.WorkHistory,
            profile.Skills,
            profile.CvUrl,
            profile.LinkedinUrl,
            profile.GithubUrl,
            profile.CreatedAt,
            profile.UpdatedAt);
    }

    public async Task<InternProfile?> GetByUserIdAsync(int userId, CancellationToken ct = default)
    {
        return await _dbSet.FirstOrDefaultAsync(p => p.UserId == userId, ct);
    }

    public async Task<IEnumerable<InternProfileDto>> GetAllDtoAsync(PaginationRequest pagination, InternProfileFilter? filter = null, CancellationToken ct = default)
    {
        var query = _dbSet.AsQueryable();

        if (!string.IsNullOrWhiteSpace(filter?.Search))
            query = query.Where(p => (p.University != null && p.University.Contains(filter.Search)) ||
                                     (p.Major != null && p.Major.Contains(filter.Search)));
        if (!string.IsNullOrWhiteSpace(filter?.University))
            query = query.Where(p => p.University == filter.University);
        if (!string.IsNullOrWhiteSpace(filter?.Major))
            query = query.Where(p => p.Major == filter.Major);
        if (filter?.GraduationYear.HasValue == true)
            query = query.Where(p => p.GraduationYear == filter.GraduationYear.Value);

        return await query
            .OrderByDescending(p => p.CreatedAt)
            .Skip((pagination.Page - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .Select(p => new InternProfileDto(
                p.InternId, p.UserId, p.DateOfBirth, p.Address, p.University, p.Major,
                p.GraduationYear, p.EducationalBackground, p.WorkHistory, p.Skills,
                p.CvUrl, p.LinkedinUrl, p.GithubUrl, p.CreatedAt, p.UpdatedAt))
            .ToListAsync(ct);
    }

    public async Task<int> CountAsync(InternProfileFilter? filter = null, CancellationToken ct = default)
    {
        var query = _dbSet.AsQueryable();

        if (!string.IsNullOrWhiteSpace(filter?.Search))
            query = query.Where(p => (p.University != null && p.University.Contains(filter.Search)) ||
                                     (p.Major != null && p.Major.Contains(filter.Search)));
        if (!string.IsNullOrWhiteSpace(filter?.University))
            query = query.Where(p => p.University == filter.University);
        if (!string.IsNullOrWhiteSpace(filter?.Major))
            query = query.Where(p => p.Major == filter.Major);
        if (filter?.GraduationYear.HasValue == true)
            query = query.Where(p => p.GraduationYear == filter.GraduationYear.Value);

        return await query.CountAsync(ct);
    }
}

public class EfInternshipCampaignRepository : GenericRepository<InternshipCampaign>, IInternshipCampaignRepository
{
    public EfInternshipCampaignRepository(AppDbContext context) : base(context) { }

    public async Task<InternshipCampaignDetailDto?> GetDetailByIdAsync(int id, CancellationToken ct = default)
    {
        var campaign = await _dbSet.FirstOrDefaultAsync(c => c.CampaignId == id, ct);
        if (campaign is null) return null;

        var createdByName = await _context.Users
            .Where(u => u.UserId == campaign.CreatedBy)
            .Select(u => u.FullName)
            .FirstOrDefaultAsync(ct);

        return new InternshipCampaignDetailDto(
            campaign.CampaignId, campaign.Title, campaign.Description, campaign.Requirements,
            campaign.NumberOfPositions, campaign.Department, campaign.Location,
            campaign.StartDate, campaign.EndDate, campaign.ApplicationDeadline,
            campaign.Status, campaign.CreatedBy, createdByName, campaign.CreatedAt, campaign.UpdatedAt);
    }

    public async Task<IEnumerable<InternshipCampaignDto>> GetAllDtoAsync(PaginationRequest pagination, InternshipCampaignFilter? filter = null, CancellationToken ct = default)
    {
        var query = _dbSet.AsQueryable();

        if (!string.IsNullOrWhiteSpace(filter?.Search))
            query = query.Where(c => c.Title.Contains(filter.Search) || (c.Description != null && c.Description.Contains(filter.Search)));
        if (!string.IsNullOrWhiteSpace(filter?.Status))
            query = query.Where(c => c.Status == filter.Status);
        if (!string.IsNullOrWhiteSpace(filter?.Department))
            query = query.Where(c => c.Department == filter.Department);
        if (!string.IsNullOrWhiteSpace(filter?.Location))
            query = query.Where(c => c.Location == filter.Location);

        return await query
            .OrderByDescending(c => c.CreatedAt)
            .Skip((pagination.Page - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .Select(c => new InternshipCampaignDto(
                c.CampaignId, c.Title, c.Description, c.Requirements, c.NumberOfPositions,
                c.Department, c.Location, c.StartDate, c.EndDate, c.ApplicationDeadline,
                c.Status, c.CreatedBy, c.CreatedAt, c.UpdatedAt))
            .ToListAsync(ct);
    }

    public async Task<int> CountAsync(InternshipCampaignFilter? filter = null, CancellationToken ct = default)
    {
        var query = _dbSet.AsQueryable();

        if (!string.IsNullOrWhiteSpace(filter?.Search))
            query = query.Where(c => c.Title.Contains(filter.Search) || (c.Description != null && c.Description.Contains(filter.Search)));
        if (!string.IsNullOrWhiteSpace(filter?.Status))
            query = query.Where(c => c.Status == filter.Status);
        if (!string.IsNullOrWhiteSpace(filter?.Department))
            query = query.Where(c => c.Department == filter.Department);
        if (!string.IsNullOrWhiteSpace(filter?.Location))
            query = query.Where(c => c.Location == filter.Location);

        return await query.CountAsync(ct);
    }
}

public class EfCampaignApplicationRepository : GenericRepository<CampaignApplication>, ICampaignApplicationRepository
{
    public EfCampaignApplicationRepository(AppDbContext context) : base(context) { }

    public async Task<CampaignApplicationDetailDto?> GetDetailByIdAsync(int id, CancellationToken ct = default)
    {
        var app = await _dbSet.FirstOrDefaultAsync(a => a.ApplicationId == id, ct);
        if (app is null) return null;

        var campaignTitle = await _context.InternshipCampaigns
            .Where(c => c.CampaignId == app.CampaignId)
            .Select(c => c.Title)
            .FirstOrDefaultAsync(ct);

        var reviewedByName = app.ReviewedBy.HasValue
            ? await _context.Users.Where(u => u.UserId == app.ReviewedBy).Select(u => u.FullName).FirstOrDefaultAsync(ct)
            : null;

        return new CampaignApplicationDetailDto(
            app.ApplicationId, app.CampaignId, campaignTitle,
            app.ApplicantEmail, app.ApplicantName, app.ApplicantPhone,
            app.CvUrl, app.CoverLetter, app.Status, app.AppliedDate,
            app.ReviewedBy, reviewedByName, app.ReviewedDate, app.Notes);
    }

    public async Task<IEnumerable<CampaignApplicationDto>> GetAllDtoAsync(PaginationRequest pagination, CampaignApplicationFilter? filter = null, CancellationToken ct = default)
    {
        var query = _dbSet.AsQueryable();

        if (!string.IsNullOrWhiteSpace(filter?.Search))
            query = query.Where(a => a.ApplicantName.Contains(filter.Search) || a.ApplicantEmail.Contains(filter.Search));
        if (!string.IsNullOrWhiteSpace(filter?.Status))
            query = query.Where(a => a.Status == filter.Status);
        if (filter?.CampaignId.HasValue == true)
            query = query.Where(a => a.CampaignId == filter.CampaignId.Value);
        if (!string.IsNullOrWhiteSpace(filter?.ApplicantEmail))
            query = query.Where(a => a.ApplicantEmail == filter.ApplicantEmail);

        return await query
            .OrderByDescending(a => a.AppliedDate)
            .Skip((pagination.Page - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .Select(a => new CampaignApplicationDto(
                a.ApplicationId, a.CampaignId,
                _context.Set<InternshipCampaign>().Where(c => c.CampaignId == a.CampaignId).Select(c => c.Title).FirstOrDefault(),
                a.ApplicantEmail, a.ApplicantName,
                a.ApplicantPhone, a.CvUrl, a.CoverLetter, a.Status, a.AppliedDate,
                a.ReviewedBy, a.ReviewedDate, a.Notes))
            .ToListAsync(ct);
    }

    public async Task<int> CountAsync(CampaignApplicationFilter? filter = null, CancellationToken ct = default)
    {
        var query = _dbSet.AsQueryable();

        if (!string.IsNullOrWhiteSpace(filter?.Search))
            query = query.Where(a => a.ApplicantName.Contains(filter.Search) || a.ApplicantEmail.Contains(filter.Search));
        if (!string.IsNullOrWhiteSpace(filter?.Status))
            query = query.Where(a => a.Status == filter.Status);
        if (filter?.CampaignId.HasValue == true)
            query = query.Where(a => a.CampaignId == filter.CampaignId.Value);
        if (!string.IsNullOrWhiteSpace(filter?.ApplicantEmail))
            query = query.Where(a => a.ApplicantEmail == filter.ApplicantEmail);

        return await query.CountAsync(ct);
    }
}

public class EfInterviewRepository : GenericRepository<Interview>, IInterviewRepository
{
    public EfInterviewRepository(AppDbContext context) : base(context) { }

    public async Task<InterviewDetailDto?> GetDetailByIdAsync(int id, CancellationToken ct = default)
    {
        var interview = await _dbSet.FirstOrDefaultAsync(i => i.InterviewId == id, ct);
        if (interview is null) return null;

        var campaignTitle = interview.CampaignId.HasValue
            ? await _context.InternshipCampaigns.Where(c => c.CampaignId == interview.CampaignId).Select(c => c.Title).FirstOrDefaultAsync(ct)
            : null;

        var internName = interview.InternId.HasValue
            ? await _context.Users.Where(u => u.UserId == interview.InternId).Select(u => u.FullName).FirstOrDefaultAsync(ct)
            : null;

        var interviewerName = await _context.Users.Where(u => u.UserId == interview.InterviewerId).Select(u => u.FullName).FirstOrDefaultAsync(ct);

        return new InterviewDetailDto(
            interview.InterviewId, interview.CampaignId, campaignTitle,
            interview.ApplicationId, interview.InternId, internName,
            interview.InterviewerId, interviewerName,
            interview.ScheduledTime, interview.DurationMinutes, interview.InterviewType,
            interview.MeetingLink, interview.Location, interview.Status,
            interview.Feedback, interview.Rating, interview.CreatedAt, interview.UpdatedAt);
    }

    public async Task<IEnumerable<InterviewDto>> GetAllDtoAsync(PaginationRequest pagination, InterviewFilter? filter = null, CancellationToken ct = default)
    {
        var query = _dbSet.AsQueryable();

        if (!string.IsNullOrWhiteSpace(filter?.Status))
            query = query.Where(i => i.Status == filter.Status);
        if (filter?.CampaignId.HasValue == true)
            query = query.Where(i => i.CampaignId == filter.CampaignId.Value);
        if (filter?.InternId.HasValue == true)
            query = query.Where(i => i.InternId == filter.InternId.Value);
        if (filter?.InterviewerId.HasValue == true)
            query = query.Where(i => i.InterviewerId == filter.InterviewerId.Value);
        if (filter?.InternId.HasValue == true)
            query = query.Where(i => i.InternId == filter.InternId.Value);
        if (filter?.FromDate.HasValue == true)
            query = query.Where(i => i.ScheduledTime >= filter.FromDate.Value);
        if (filter?.ToDate.HasValue == true)
            query = query.Where(i => i.ScheduledTime <= filter.ToDate.Value);

        return await query
            .OrderByDescending(i => i.ScheduledTime)
            .Skip((pagination.Page - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .Select(i => new InterviewDto(
                i.InterviewId, i.CampaignId,
                _context.Set<InternshipCampaign>().Where(c => c.CampaignId == i.CampaignId).Select(c => c.Title).FirstOrDefault(),
                i.ApplicationId,
                _context.Set<CampaignApplication>().Where(a => a.ApplicationId == i.ApplicationId).Select(a => a.ApplicantEmail).FirstOrDefault(),
                _context.Set<CampaignApplication>().Where(a => a.ApplicationId == i.ApplicationId).Select(a => a.ApplicantName).FirstOrDefault(),
                i.InternId,
                i.InterviewerId,
                _context.Users.Where(u => u.UserId == i.InterviewerId).Select(u => u.FullName).FirstOrDefault(),
                i.ScheduledTime, i.DurationMinutes, i.InterviewType,
                i.MeetingLink, i.Location, i.Status, i.Feedback, i.Rating,
                i.CreatedAt, i.UpdatedAt))
            .ToListAsync(ct);
    }

    public async Task<int> CountAsync(InterviewFilter? filter = null, CancellationToken ct = default)
    {
        var query = _dbSet.AsQueryable();

        if (!string.IsNullOrWhiteSpace(filter?.Status))
            query = query.Where(i => i.Status == filter.Status);
        if (filter?.CampaignId.HasValue == true)
            query = query.Where(i => i.CampaignId == filter.CampaignId.Value);
        if (filter?.InternId.HasValue == true)
            query = query.Where(i => i.InternId == filter.InternId.Value);
        if (filter?.InterviewerId.HasValue == true)
            query = query.Where(i => i.InterviewerId == filter.InterviewerId.Value);
        if (filter?.FromDate.HasValue == true)
            query = query.Where(i => i.ScheduledTime >= filter.FromDate.Value);
        if (filter?.ToDate.HasValue == true)
            query = query.Where(i => i.ScheduledTime <= filter.ToDate.Value);
        if (!string.IsNullOrWhiteSpace(filter?.ApplicantEmail))
        {
            var appIds = _context.Set<CampaignApplication>()
                .Where(a => a.ApplicantEmail == filter.ApplicantEmail)
                .Select(a => a.ApplicationId);
            query = query.Where(i => i.ApplicationId.HasValue && appIds.Contains(i.ApplicationId.Value));
        }

        return await query.CountAsync(ct);
    }
}

public class EfTrainingProgramRepository : GenericRepository<TrainingProgram>, ITrainingProgramRepository
{
    public EfTrainingProgramRepository(AppDbContext context) : base(context) { }

    public async Task<TrainingProgramDetailDto?> GetDetailByIdAsync(int id, CancellationToken ct = default)
    {
        var program = await _dbSet.FirstOrDefaultAsync(p => p.ProgramId == id, ct);
        if (program is null) return null;

        var coordinatorName = await _context.Users
            .Where(u => u.UserId == program.CoordinatorId)
            .Select(u => u.FullName)
            .FirstOrDefaultAsync(ct);

        return new TrainingProgramDetailDto(
            program.ProgramId, program.ProgramName, program.Description, program.Objectives,
            program.DurationWeeks, program.StartDate, program.EndDate,
            program.CoordinatorId, coordinatorName ?? "",
            program.Status, program.CreatedAt, program.UpdatedAt);
    }

    public async Task<IEnumerable<TrainingProgramDto>> GetAllDtoAsync(PaginationRequest pagination, TrainingProgramFilter? filter = null, CancellationToken ct = default)
    {
        var query = _dbSet.AsQueryable();

        if (!string.IsNullOrWhiteSpace(filter?.Search))
            query = query.Where(p => p.ProgramName.Contains(filter.Search) || (p.Description != null && p.Description.Contains(filter.Search)));
        if (!string.IsNullOrWhiteSpace(filter?.Status))
            query = query.Where(p => p.Status == filter.Status);
        if (filter?.CoordinatorId.HasValue == true)
            query = query.Where(p => p.CoordinatorId == filter.CoordinatorId.Value);

        return await query
            .OrderByDescending(p => p.CreatedAt)
            .Skip((pagination.Page - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .Select(p => new TrainingProgramDto(
                p.ProgramId, p.ProgramName, p.Description, p.Objectives,
                p.DurationWeeks, p.StartDate, p.EndDate, p.CoordinatorId,
                p.Status, p.CreatedAt, p.UpdatedAt))
            .ToListAsync(ct);
    }

    public async Task<int> CountAsync(TrainingProgramFilter? filter = null, CancellationToken ct = default)
    {
        var query = _dbSet.AsQueryable();

        if (!string.IsNullOrWhiteSpace(filter?.Search))
            query = query.Where(p => p.ProgramName.Contains(filter.Search) || (p.Description != null && p.Description.Contains(filter.Search)));
        if (!string.IsNullOrWhiteSpace(filter?.Status))
            query = query.Where(p => p.Status == filter.Status);
        if (filter?.CoordinatorId.HasValue == true)
            query = query.Where(p => p.CoordinatorId == filter.CoordinatorId.Value);

        return await query.CountAsync(ct);
    }
}

public class EfLearningResourceRepository : GenericRepository<LearningResource>, ILearningResourceRepository
{
    public EfLearningResourceRepository(AppDbContext context) : base(context) { }

    public async Task<LearningResourceDetailDto?> GetDetailByIdAsync(int id, CancellationToken ct = default)
    {
        var resource = await _dbSet.FirstOrDefaultAsync(r => r.ResourceId == id, ct);
        if (resource is null) return null;

        var programName = await _context.TrainingPrograms
            .Where(p => p.ProgramId == resource.ProgramId)
            .Select(p => p.ProgramName)
            .FirstOrDefaultAsync(ct);

        var uploadedByName = await _context.Users
            .Where(u => u.UserId == resource.UploadedBy)
            .Select(u => u.FullName)
            .FirstOrDefaultAsync(ct);

        return new LearningResourceDetailDto(
            resource.ResourceId, resource.ProgramId, programName ?? "",
            resource.Title, resource.Description, resource.ResourceUrl,
            resource.ResourceType, resource.FileSizeMb,
            resource.UploadedBy, uploadedByName ?? "", resource.UploadedAt);
    }

    public async Task<IEnumerable<LearningResourceDto>> GetAllDtoAsync(PaginationRequest pagination, LearningResourceFilter? filter = null, CancellationToken ct = default)
    {
        var query = _dbSet.AsQueryable();

        if (!string.IsNullOrWhiteSpace(filter?.Search))
            query = query.Where(r => r.Title.Contains(filter.Search) || (r.Description != null && r.Description.Contains(filter.Search)));
        if (!string.IsNullOrWhiteSpace(filter?.ResourceType))
            query = query.Where(r => r.ResourceType == filter.ResourceType);
        if (filter?.ProgramId.HasValue == true)
            query = query.Where(r => r.ProgramId == filter.ProgramId.Value);

        return await query
            .OrderByDescending(r => r.UploadedAt)
            .Skip((pagination.Page - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .Select(r => new LearningResourceDto(
                r.ResourceId, r.ProgramId, r.Title, r.Description,
                r.ResourceUrl, r.ResourceType, r.FileSizeMb, r.UploadedBy, r.UploadedAt))
            .ToListAsync(ct);
    }

    public async Task<int> CountAsync(LearningResourceFilter? filter = null, CancellationToken ct = default)
    {
        var query = _dbSet.AsQueryable();

        if (!string.IsNullOrWhiteSpace(filter?.Search))
            query = query.Where(r => r.Title.Contains(filter.Search) || (r.Description != null && r.Description.Contains(filter.Search)));
        if (!string.IsNullOrWhiteSpace(filter?.ResourceType))
            query = query.Where(r => r.ResourceType == filter.ResourceType);
        if (filter?.ProgramId.HasValue == true)
            query = query.Where(r => r.ProgramId == filter.ProgramId.Value);

        return await query.CountAsync(ct);
    }
}

public class EfMentorshipRepository : GenericRepository<Mentorship>, IMentorshipRepository
{
    public EfMentorshipRepository(AppDbContext context) : base(context) { }

    public async Task<MentorshipDetailDto?> GetDetailByIdAsync(int id, CancellationToken ct = default)
    {
        var mentorship = await _dbSet.FirstOrDefaultAsync(m => m.MentorshipId == id, ct);
        if (mentorship is null) return null;

        var mentorName = await _context.Users.Where(u => u.UserId == mentorship.MentorId).Select(u => u.FullName).FirstOrDefaultAsync(ct);
        var internName = await _context.Users.Where(u => u.UserId == mentorship.InternId).Select(u => u.FullName).FirstOrDefaultAsync(ct);
        var programName = await _context.TrainingPrograms.Where(p => p.ProgramId == mentorship.ProgramId).Select(p => p.ProgramName).FirstOrDefaultAsync(ct);

        return new MentorshipDetailDto(
            mentorship.MentorshipId, mentorship.MentorId, mentorName ?? "",
            mentorship.InternId, internName ?? "", mentorship.ProgramId, programName ?? "",
            mentorship.StartDate, mentorship.EndDate, mentorship.Status, mentorship.CreatedAt);
    }

    public async Task<IEnumerable<MentorshipDto>> GetAllDtoAsync(PaginationRequest pagination, MentorshipFilter? filter = null, CancellationToken ct = default)
    {
        var query = _dbSet
            .Include(m => m.Mentor)
            .Include(m => m.Intern)
            .Include(m => m.Program)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(filter?.Status))
            query = query.Where(m => m.Status == filter.Status);
        if (filter?.MentorId.HasValue == true)
            query = query.Where(m => m.MentorId == filter.MentorId.Value);
        if (filter?.InternId.HasValue == true)
            query = query.Where(m => m.InternId == filter.InternId.Value);
        if (filter?.ProgramId.HasValue == true)
            query = query.Where(m => m.ProgramId == filter.ProgramId.Value);

        return await query
            .OrderByDescending(m => m.CreatedAt)
            .Skip((pagination.Page - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .Select(m => new MentorshipDto(
                m.MentorshipId, m.MentorId, m.Mentor!.FullName,
                m.InternId, m.Intern!.FullName,
                m.ProgramId, m.Program!.ProgramName,
                m.StartDate, m.EndDate, m.Status, m.CreatedAt))
            .ToListAsync(ct);
    }

    public async Task<int> CountAsync(MentorshipFilter? filter = null, CancellationToken ct = default)
    {
        var query = _dbSet.AsQueryable();

        if (!string.IsNullOrWhiteSpace(filter?.Status))
            query = query.Where(m => m.Status == filter.Status);
        if (filter?.MentorId.HasValue == true)
            query = query.Where(m => m.MentorId == filter.MentorId.Value);
        if (filter?.InternId.HasValue == true)
            query = query.Where(m => m.InternId == filter.InternId.Value);
        if (filter?.ProgramId.HasValue == true)
            query = query.Where(m => m.ProgramId == filter.ProgramId.Value);

        return await query.CountAsync(ct);
    }
}

public class EfTaskItemRepository : GenericRepository<TaskItem>, ITaskItemRepository
{
    public EfTaskItemRepository(AppDbContext context) : base(context) { }

    public async Task<TaskItemDetailDto?> GetDetailByIdAsync(int id, CancellationToken ct = default)
    {
        var task = await _dbSet.FirstOrDefaultAsync(t => t.TaskId == id, ct);
        if (task is null) return null;

        var internName = await _context.Users.Where(u => u.UserId == task.InternId).Select(u => u.FullName).FirstOrDefaultAsync(ct);
        var assignedByName = await _context.Users.Where(u => u.UserId == task.AssignedBy).Select(u => u.FullName).FirstOrDefaultAsync(ct);
        var programName = task.ProgramId.HasValue
            ? await _context.TrainingPrograms.Where(p => p.ProgramId == task.ProgramId).Select(p => p.ProgramName).FirstOrDefaultAsync(ct)
            : null;

        return new TaskItemDetailDto(
            task.TaskId, task.InternId, internName ?? "",
            task.AssignedBy, assignedByName ?? "",
            task.ProgramId, programName,
            task.Title, task.Description, task.DueDate,
            task.Priority, task.Status, task.CompletionDate,
            task.CreatedAt, task.UpdatedAt);
    }

    public async Task<IEnumerable<TaskItemDto>> GetAllDtoAsync(PaginationRequest pagination, TaskItemFilter? filter = null, CancellationToken ct = default)
    {
        var query = _dbSet.AsQueryable();

        if (!string.IsNullOrWhiteSpace(filter?.Search))
            query = query.Where(t => t.Title.Contains(filter.Search) || (t.Description != null && t.Description.Contains(filter.Search)));
        if (!string.IsNullOrWhiteSpace(filter?.Status))
            query = query.Where(t => t.Status == filter.Status);
        if (!string.IsNullOrWhiteSpace(filter?.Priority))
            query = query.Where(t => t.Priority == filter.Priority);
        if (filter?.InternId.HasValue == true)
            query = query.Where(t => t.InternId == filter.InternId.Value);
        if (filter?.AssignedBy.HasValue == true)
            query = query.Where(t => t.AssignedBy == filter.AssignedBy.Value);
        if (filter?.ProgramId.HasValue == true)
            query = query.Where(t => t.ProgramId == filter.ProgramId.Value);

        return await query
            .OrderByDescending(t => t.CreatedAt)
            .Skip((pagination.Page - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .Select(t => new TaskItemDto(
                t.TaskId, t.InternId,
                _context.Users.Where(u => u.UserId == t.InternId).Select(u => u.FullName).FirstOrDefault(),
                t.AssignedBy,
                _context.Users.Where(u => u.UserId == t.AssignedBy).Select(u => u.FullName).FirstOrDefault(),
                t.ProgramId,
                t.ProgramId.HasValue ? _context.TrainingPrograms.Where(p => p.ProgramId == t.ProgramId).Select(p => p.ProgramName).FirstOrDefault() : null,
                t.Title, t.Description, t.DueDate, t.Priority, t.Status, t.CompletionDate,
                t.CreatedAt, t.UpdatedAt))
            .ToListAsync(ct);
    }

    public async Task<int> CountAsync(TaskItemFilter? filter = null, CancellationToken ct = default)
    {
        var query = _dbSet.AsQueryable();

        if (!string.IsNullOrWhiteSpace(filter?.Search))
            query = query.Where(t => t.Title.Contains(filter.Search) || (t.Description != null && t.Description.Contains(filter.Search)));
        if (!string.IsNullOrWhiteSpace(filter?.Status))
            query = query.Where(t => t.Status == filter.Status);
        if (filter?.InternId.HasValue == true)
            query = query.Where(t => t.InternId == filter.InternId.Value);

        return await query.CountAsync(ct);
    }
}

public class EfTaskSubmissionRepository : GenericRepository<TaskSubmission>, ITaskSubmissionRepository
{
    public EfTaskSubmissionRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<TaskSubmissionDto>> GetByTaskIdAsync(int taskId, CancellationToken ct = default)
    {
        return await _dbSet
            .Where(s => s.TaskId == taskId)
            .OrderByDescending(s => s.SubmittedAt)
            .Select(s => new TaskSubmissionDto(
                s.SubmissionId, s.TaskId,
                _context.Tasks.Where(t => t.TaskId == s.TaskId).Select(t => t.Title).FirstOrDefault(),
                s.InternId,
                _context.Users.Where(u => u.UserId == s.InternId).Select(u => u.FullName).FirstOrDefault(),
                s.SubmissionUrl, s.SubmissionText, s.Comments, s.Status,
                s.GradedBy,
                s.GradedBy.HasValue ? _context.Users.Where(u => u.UserId == s.GradedBy).Select(u => u.FullName).FirstOrDefault() : null,
                s.Score, s.Feedback,
                s.SubmittedAt, s.GradedAt))
            .ToListAsync(ct);
    }

    public new async Task<TaskSubmissionDto?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var s = await _dbSet.FirstOrDefaultAsync(s => s.SubmissionId == id, ct);
        if (s is null) return null;

        return s.ToDto(
            await _context.Tasks.Where(t => t.TaskId == s.TaskId).Select(t => t.Title).FirstOrDefaultAsync(ct),
            await _context.Users.Where(u => u.UserId == s.InternId).Select(u => u.FullName).FirstOrDefaultAsync(ct),
            s.GradedBy.HasValue ? await _context.Users.Where(u => u.UserId == s.GradedBy).Select(u => u.FullName).FirstOrDefaultAsync(ct) : null);
    }
}

public class EfDailyLogRepository : GenericRepository<DailyLog>, IDailyLogRepository
{
    public EfDailyLogRepository(AppDbContext context) : base(context) { }

    public async Task<DailyLogDetailDto?> GetDetailByIdAsync(int id, CancellationToken ct = default)
    {
        var log = await _dbSet.FirstOrDefaultAsync(l => l.LogId == id, ct);
        if (log is null) return null;

        var internName = await _context.Users.Where(u => u.UserId == log.InternId).Select(u => u.FullName).FirstOrDefaultAsync(ct);
        var mentorName = log.MentorId.HasValue
            ? await _context.Users.Where(u => u.UserId == log.MentorId).Select(u => u.FullName).FirstOrDefaultAsync(ct)
            : null;

        return new DailyLogDetailDto(
            log.LogId, log.InternId, internName ?? "",
            log.MentorId, mentorName,
            log.LogDate, log.ActivityDescription,
            log.HoursWorked, log.ChallengesFaced,
            log.MentorFeedback, log.KpiScore,
            log.CreatedAt, log.UpdatedAt);
    }

    public async Task<IEnumerable<DailyLogDto>> GetAllDtoAsync(PaginationRequest pagination, DailyLogFilter? filter = null, CancellationToken ct = default)
    {
        var query = _dbSet.AsQueryable();

        if (!string.IsNullOrWhiteSpace(filter?.Search))
            query = query.Where(l => l.ActivityDescription.Contains(filter.Search));
        if (filter?.InternId.HasValue == true)
            query = query.Where(l => l.InternId == filter.InternId.Value);
        if (filter?.MentorId.HasValue == true)
            query = query.Where(l => l.MentorId == filter.MentorId.Value);
        if (filter?.FromDate.HasValue == true)
            query = query.Where(l => l.LogDate >= filter.FromDate.Value);
        if (filter?.ToDate.HasValue == true)
            query = query.Where(l => l.LogDate <= filter.ToDate.Value);

        return await query
            .OrderByDescending(l => l.LogDate)
            .Skip((pagination.Page - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .Select(l => new DailyLogDto(
                l.LogId, l.InternId, l.MentorId, l.LogDate, l.ActivityDescription,
                l.HoursWorked, l.ChallengesFaced, l.MentorFeedback, l.KpiScore,
                l.CreatedAt, l.UpdatedAt))
            .ToListAsync(ct);
    }

    public async Task<int> CountAsync(DailyLogFilter? filter = null, CancellationToken ct = default)
    {
        var query = _dbSet.AsQueryable();

        if (!string.IsNullOrWhiteSpace(filter?.Search))
            query = query.Where(l => l.ActivityDescription.Contains(filter.Search));
        if (filter?.InternId.HasValue == true)
            query = query.Where(l => l.InternId == filter.InternId.Value);
        if (filter?.FromDate.HasValue == true)
            query = query.Where(l => l.LogDate >= filter.FromDate.Value);
        if (filter?.ToDate.HasValue == true)
            query = query.Where(l => l.LogDate <= filter.ToDate.Value);

        return await query.CountAsync(ct);
    }
}

public class EfAssessmentRepository : GenericRepository<Assessment>, IAssessmentRepository
{
    public EfAssessmentRepository(AppDbContext context) : base(context) { }

    public async Task<AssessmentDetailDto?> GetDetailByIdAsync(int id, CancellationToken ct = default)
    {
        var assessment = await _dbSet.FirstOrDefaultAsync(a => a.AssessmentId == id, ct);
        if (assessment is null) return null;

        var internName = await _context.Users.Where(u => u.UserId == assessment.InternId).Select(u => u.FullName).FirstOrDefaultAsync(ct);
        var mentorName = await _context.Users.Where(u => u.UserId == assessment.MentorId).Select(u => u.FullName).FirstOrDefaultAsync(ct);
        var programName = assessment.ProgramId.HasValue
            ? await _context.TrainingPrograms.Where(p => p.ProgramId == assessment.ProgramId).Select(p => p.ProgramName).FirstOrDefaultAsync(ct)
            : null;

        return new AssessmentDetailDto(
            assessment.AssessmentId, assessment.InternId, internName ?? "",
            assessment.MentorId, mentorName ?? "",
            assessment.ProgramId, programName,
            assessment.AssessmentDate, assessment.AssessmentType,
            assessment.TechnicalSkillsScore, assessment.SoftSkillsScore,
            assessment.CommunicationScore, assessment.TeamworkScore,
            assessment.OverallRating, assessment.Strengths,
            assessment.AreasForImprovement, assessment.Comments,
            assessment.CreatedAt);
    }

    public async Task<IEnumerable<AssessmentDto>> GetAllDtoAsync(PaginationRequest pagination, AssessmentFilter? filter = null, CancellationToken ct = default)
    {
        var query = _dbSet.AsQueryable();

        if (!string.IsNullOrWhiteSpace(filter?.AssessmentType))
            query = query.Where(a => a.AssessmentType == filter.AssessmentType);
        if (filter?.InternId.HasValue == true)
            query = query.Where(a => a.InternId == filter.InternId.Value);
        if (filter?.MentorId.HasValue == true)
            query = query.Where(a => a.MentorId == filter.MentorId.Value);
        if (filter?.ProgramId.HasValue == true)
            query = query.Where(a => a.ProgramId == filter.ProgramId.Value);
        if (filter?.FromDate.HasValue == true)
            query = query.Where(a => a.AssessmentDate >= filter.FromDate.Value);
        if (filter?.ToDate.HasValue == true)
            query = query.Where(a => a.AssessmentDate <= filter.ToDate.Value);

        return await query
            .OrderByDescending(a => a.AssessmentDate)
            .Skip((pagination.Page - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .Select(a => new AssessmentDto(
                a.AssessmentId, a.InternId, a.MentorId, a.ProgramId,
                a.AssessmentDate, a.AssessmentType, a.TechnicalSkillsScore,
                a.SoftSkillsScore, a.CommunicationScore, a.TeamworkScore,
                a.OverallRating, a.Strengths, a.AreasForImprovement, a.Comments,
                a.CreatedAt))
            .ToListAsync(ct);
    }

    public async Task<int> CountAsync(AssessmentFilter? filter = null, CancellationToken ct = default)
    {
        var query = _dbSet.AsQueryable();

        if (!string.IsNullOrWhiteSpace(filter?.AssessmentType))
            query = query.Where(a => a.AssessmentType == filter.AssessmentType);
        if (filter?.InternId.HasValue == true)
            query = query.Where(a => a.InternId == filter.InternId.Value);
        if (filter?.FromDate.HasValue == true)
            query = query.Where(a => a.AssessmentDate >= filter.FromDate.Value);
        if (filter?.ToDate.HasValue == true)
            query = query.Where(a => a.AssessmentDate <= filter.ToDate.Value);

        return await query.CountAsync(ct);
    }
}

public class EfFeedbackRepository : GenericRepository<Feedback>, IFeedbackRepository
{
    public EfFeedbackRepository(AppDbContext context) : base(context) { }

    public async Task<FeedbackDetailDto?> GetDetailByIdAsync(int id, CancellationToken ct = default)
    {
        var feedback = await _dbSet.FirstOrDefaultAsync(f => f.FeedbackId == id, ct);
        if (feedback is null) return null;

        var internName = await _context.Users.Where(u => u.UserId == feedback.InternId).Select(u => u.FullName).FirstOrDefaultAsync(ct);

        return new FeedbackDetailDto(
            feedback.FeedbackId, feedback.InternId, internName ?? "",
            feedback.FeedbackType, feedback.RelatedId, null,
            feedback.Rating, feedback.Comments,
            feedback.IsAnonymous, feedback.SubmittedAt);
    }

    public async Task<IEnumerable<FeedbackDto>> GetAllDtoAsync(PaginationRequest pagination, FeedbackFilter? filter = null, CancellationToken ct = default)
    {
        var query = _dbSet.AsQueryable();

        if (!string.IsNullOrWhiteSpace(filter?.FeedbackType))
            query = query.Where(f => f.FeedbackType == filter.FeedbackType);
        if (filter?.InternId.HasValue == true)
            query = query.Where(f => f.InternId == filter.InternId.Value);
        if (filter?.IsAnonymous.HasValue == true)
            query = query.Where(f => f.IsAnonymous == filter.IsAnonymous.Value);
        if (filter?.FromDate.HasValue == true)
            query = query.Where(f => f.SubmittedAt >= filter.FromDate.Value);
        if (filter?.ToDate.HasValue == true)
            query = query.Where(f => f.SubmittedAt <= filter.ToDate.Value);

        return await query
            .OrderByDescending(f => f.SubmittedAt)
            .Skip((pagination.Page - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .Select(f => new FeedbackDto(
                f.FeedbackId, f.InternId, f.FeedbackType, f.RelatedId,
                f.Rating, f.Comments, f.IsAnonymous, f.SubmittedAt))
            .ToListAsync(ct);
    }

    public async Task<int> CountAsync(FeedbackFilter? filter = null, CancellationToken ct = default)
    {
        var query = _dbSet.AsQueryable();

        if (!string.IsNullOrWhiteSpace(filter?.FeedbackType))
            query = query.Where(f => f.FeedbackType == filter.FeedbackType);
        if (filter?.InternId.HasValue == true)
            query = query.Where(f => f.InternId == filter.InternId.Value);
        if (filter?.FromDate.HasValue == true)
            query = query.Where(f => f.SubmittedAt >= filter.FromDate.Value);
        if (filter?.ToDate.HasValue == true)
            query = query.Where(f => f.SubmittedAt <= filter.ToDate.Value);

        return await query.CountAsync(ct);
    }
}

public class EfCommunicationRepository : GenericRepository<Communication>, ICommunicationRepository
{
    public EfCommunicationRepository(AppDbContext context) : base(context) { }

    public async Task<CommunicationDetailDto?> GetDetailByIdAsync(int id, CancellationToken ct = default)
    {
        var comm = await _dbSet.FirstOrDefaultAsync(c => c.MessageId == id, ct);
        if (comm is null) return null;

        var senderName = await _context.Users.Where(u => u.UserId == comm.SenderId).Select(u => u.FullName).FirstOrDefaultAsync(ct);
        var receiverName = await _context.Users.Where(u => u.UserId == comm.ReceiverId).Select(u => u.FullName).FirstOrDefaultAsync(ct);

        return new CommunicationDetailDto(
            comm.MessageId, comm.SenderId, senderName ?? "",
            comm.ReceiverId, receiverName ?? "",
            comm.Subject, comm.MessageContent,
            comm.IsRead, comm.ParentMessageId,
            comm.SentAt, comm.ReadAt);
    }

    public async Task<IEnumerable<CommunicationDto>> GetAllDtoAsync(PaginationRequest pagination, CommunicationFilter? filter = null, CancellationToken ct = default)
    {
        var query = _dbSet.AsQueryable();

        if (filter?.SenderId.HasValue == true)
            query = query.Where(c => c.SenderId == filter.SenderId.Value);
        if (filter?.ReceiverId.HasValue == true)
            query = query.Where(c => c.ReceiverId == filter.ReceiverId.Value);
        if (filter?.IsRead.HasValue == true)
            query = query.Where(c => c.IsRead == filter.IsRead.Value);

        return await query
            .OrderByDescending(c => c.SentAt)
            .Skip((pagination.Page - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .Select(c => new CommunicationDto(
                c.MessageId, c.SenderId, c.ReceiverId, c.Subject,
                c.MessageContent, c.IsRead, c.ParentMessageId,
                c.SentAt, c.ReadAt))
            .ToListAsync(ct);
    }

    public async Task<int> CountAsync(CommunicationFilter? filter = null, CancellationToken ct = default)
    {
        var query = _dbSet.AsQueryable();

        if (filter?.SenderId.HasValue == true)
            query = query.Where(c => c.SenderId == filter.SenderId.Value);
        if (filter?.ReceiverId.HasValue == true)
            query = query.Where(c => c.ReceiverId == filter.ReceiverId.Value);
        if (filter?.IsRead.HasValue == true)
            query = query.Where(c => c.IsRead == filter.IsRead.Value);

        return await query.CountAsync(ct);
    }

    public async Task<IEnumerable<ChatContactDto>> GetConversationsAsync(int userId, CancellationToken ct = default)
    {
        var allMessages = _dbSet
            .Where(c => c.SenderId == userId || c.ReceiverId == userId)
            .GroupBy(c => c.SenderId == userId ? c.ReceiverId : c.SenderId)
            .Select(g => new {
                UserId = g.Key,
                LastMessage = g.OrderByDescending(c => c.SentAt).Select(c => c.MessageContent).FirstOrDefault()!,
                LastMessageTime = g.Max(c => c.SentAt),
                UnreadCount = g.Count(c =>
                    c.ReceiverId == userId && !c.IsRead)
            })
            .ToList();

        var userIds = allMessages.Select(x => x.UserId).ToList();
        var users = await _context.Users
            .Where(u => userIds.Contains(u.UserId))
            .Select(u => new { u.UserId, u.FullName, u.Email, u.AvatarUrl, u.RoleId })
            .ToListAsync(ct);

        var roles = await _context.Roles.ToDictionaryAsync(r => r.RoleId);

        return allMessages.Select(x => {
            var user = users.FirstOrDefault(u => u.UserId == x.UserId);
            return new ChatContactDto(
                x.UserId,
                user?.FullName ?? "Unknown",
                user?.Email ?? "",
                user?.AvatarUrl,
                user?.RoleId ?? 0,
                user != null && roles.TryGetValue(user.RoleId, out var role) ? role.RoleName : null,
                x.LastMessage,
                x.LastMessageTime,
                x.UnreadCount);
        }).OrderByDescending(x => x.LastMessageTime);
    }

    public async Task<IEnumerable<CommunicationDto>> GetMessagesBetweenUsersAsync(int? userId, int otherUserId, PaginationRequest pagination, CancellationToken ct = default)
    {
        var query = _dbSet.AsQueryable();

        if (userId.HasValue)
        {
            query = query.Where(c =>
                (c.SenderId == userId.Value && c.ReceiverId == otherUserId) ||
                (c.SenderId == otherUserId && c.ReceiverId == userId.Value));
        }
        else
        {
            query = query.Where(c =>
                c.SenderId == otherUserId || c.ReceiverId == otherUserId);
        }

        return await query
            .OrderByDescending(c => c.SentAt)
            .Skip((pagination.Page - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .Select(c => new CommunicationDto(
                c.MessageId, c.SenderId, c.ReceiverId, c.Subject,
                c.MessageContent, c.IsRead, c.ParentMessageId,
                c.SentAt, c.ReadAt))
            .ToListAsync(ct);
    }

    public async Task MarkAllAsReadAsync(int senderId, int receiverId, CancellationToken ct = default)
    {
        var unread = await _dbSet
            .Where(c => c.SenderId == senderId && c.ReceiverId == receiverId && !c.IsRead)
            .ToListAsync(ct);

        foreach (var msg in unread)
        {
            msg.IsRead = true;
            msg.ReadAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync(ct);
    }
}

public class EfNotificationRepository : GenericRepository<Notification>, INotificationRepository
{
    public EfNotificationRepository(AppDbContext context) : base(context) { }

    public async Task<NotificationDetailDto?> GetDetailByIdAsync(int id, CancellationToken ct = default)
    {
        var notification = await _dbSet.FirstOrDefaultAsync(n => n.NotificationId == id, ct);
        if (notification is null) return null;

        var userName = await _context.Users.Where(u => u.UserId == notification.UserId).Select(u => u.FullName).FirstOrDefaultAsync(ct);

        return new NotificationDetailDto(
            notification.NotificationId, notification.UserId, userName ?? "",
            notification.NotificationType, notification.Category,
            notification.Subject, notification.Content,
            notification.RelatedId, notification.RelatedType,
            notification.IsRead, notification.SentAt, notification.ReadAt);
    }

    public async Task<IEnumerable<NotificationDto>> GetAllDtoAsync(PaginationRequest pagination, NotificationFilter? filter = null, CancellationToken ct = default)
    {
        var query = _dbSet.AsQueryable();

        if (!string.IsNullOrWhiteSpace(filter?.Category))
            query = query.Where(n => n.Category == filter.Category);
        if (filter?.UserId.HasValue == true)
            query = query.Where(n => n.UserId == filter.UserId.Value);
        if (filter?.IsRead.HasValue == true)
            query = query.Where(n => n.IsRead == filter.IsRead.Value);

        return await query
            .OrderByDescending(n => n.SentAt)
            .Skip((pagination.Page - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .Select(n => new NotificationDto(
                n.NotificationId, n.UserId, n.NotificationType, n.Category,
                n.Subject, n.Content, n.RelatedId, n.RelatedType,
                n.IsRead, n.SentAt, n.ReadAt))
            .ToListAsync(ct);
    }

    public async Task<int> CountAsync(NotificationFilter? filter = null, CancellationToken ct = default)
    {
        var query = _dbSet.AsQueryable();

        if (!string.IsNullOrWhiteSpace(filter?.Category))
            query = query.Where(n => n.Category == filter.Category);
        if (filter?.UserId.HasValue == true)
            query = query.Where(n => n.UserId == filter.UserId.Value);
        if (filter?.IsRead.HasValue == true)
            query = query.Where(n => n.IsRead == filter.IsRead.Value);

        return await query.CountAsync(ct);
    }
}

public class EfReportRepository : GenericRepository<Report>, IReportRepository
{
    public EfReportRepository(AppDbContext context) : base(context) { }

    public async Task<ReportDetailDto?> GetDetailByIdAsync(int id, CancellationToken ct = default)
    {
        var report = await _dbSet.FirstOrDefaultAsync(r => r.ReportId == id, ct);
        if (report is null) return null;

        var generatedByName = await _context.Users.Where(u => u.UserId == report.GeneratedBy).Select(u => u.FullName).FirstOrDefaultAsync(ct);

        return new ReportDetailDto(
            report.ReportId, report.ReportName, report.ReportType,
            report.Description, report.GeneratedBy, generatedByName ?? "",
            report.FileUrl, report.FileFormat, report.Parameters,
            report.GeneratedAt);
    }

    public async Task<IEnumerable<ReportDto>> GetAllDtoAsync(PaginationRequest pagination, ReportFilter? filter = null, CancellationToken ct = default)
    {
        var query = _dbSet.AsQueryable();

        if (!string.IsNullOrWhiteSpace(filter?.Search))
            query = query.Where(r => r.ReportName.Contains(filter.Search) || (r.Description != null && r.Description.Contains(filter.Search)));
        if (!string.IsNullOrWhiteSpace(filter?.ReportType))
            query = query.Where(r => r.ReportType == filter.ReportType);

        return await query
            .OrderByDescending(r => r.GeneratedAt)
            .Skip((pagination.Page - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .Select(r => new ReportDto(
                r.ReportId, r.ReportName, r.ReportType, r.Description,
                r.GeneratedBy, r.FileUrl, r.FileFormat, r.Parameters,
                r.GeneratedAt))
            .ToListAsync(ct);
    }

    public async Task<int> CountAsync(ReportFilter? filter = null, CancellationToken ct = default)
    {
        var query = _dbSet.AsQueryable();

        if (!string.IsNullOrWhiteSpace(filter?.Search))
            query = query.Where(r => r.ReportName.Contains(filter.Search));
        if (!string.IsNullOrWhiteSpace(filter?.ReportType))
            query = query.Where(r => r.ReportType == filter.ReportType);

        return await query.CountAsync(ct);
    }
}

public class EfAttendanceRepository : GenericRepository<Attendance>, IAttendanceRepository
{
    public EfAttendanceRepository(AppDbContext context) : base(context) { }

    public async Task<AttendanceDetailDto?> GetDetailByIdAsync(int id, CancellationToken ct = default)
    {
        var attendance = await _dbSet.FirstOrDefaultAsync(a => a.AttendanceId == id, ct);
        if (attendance is null) return null;

        var internName = await _context.Users.Where(u => u.UserId == attendance.InternId).Select(u => u.FullName).FirstOrDefaultAsync(ct);
        var approvedByName = attendance.ApprovedBy.HasValue
            ? await _context.Users.Where(u => u.UserId == attendance.ApprovedBy).Select(u => u.FullName).FirstOrDefaultAsync(ct)
            : null;

        return new AttendanceDetailDto(
            attendance.AttendanceId, attendance.InternId, internName ?? "",
            attendance.AttendanceDate, attendance.CheckInTime,
            attendance.CheckOutTime, attendance.Status,
            attendance.Notes, attendance.ApprovedBy, approvedByName,
            attendance.CreatedAt);
    }

    public async Task<IEnumerable<AttendanceDto>> GetAllDtoAsync(PaginationRequest pagination, AttendanceFilter? filter = null, CancellationToken ct = default)
    {
        var query = _dbSet.AsQueryable();

        if (filter?.InternId.HasValue == true)
            query = query.Where(a => a.InternId == filter.InternId.Value);
        if (!string.IsNullOrWhiteSpace(filter?.Status))
            query = query.Where(a => a.Status == filter.Status);
        if (filter?.FromDate.HasValue == true)
            query = query.Where(a => a.AttendanceDate >= filter.FromDate.Value);
        if (filter?.ToDate.HasValue == true)
            query = query.Where(a => a.AttendanceDate <= filter.ToDate.Value);

        var results = await query
            .OrderByDescending(a => a.AttendanceDate)
            .Skip((pagination.Page - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .ToListAsync(ct);

        var userIds = results.Select(a => a.InternId).Distinct().ToList();
        var users = await _context.Users
            .Where(u => userIds.Contains(u.UserId))
            .ToDictionaryAsync(u => u.UserId, u => u.FullName, ct);

        return results.Select(a => new AttendanceDto(
            a.AttendanceId, a.InternId,
            users.GetValueOrDefault(a.InternId) ?? $"Intern {a.InternId}",
            a.AttendanceDate, a.CheckInTime, a.CheckOutTime, a.Status,
            a.Notes, a.ApprovedBy, a.CreatedAt));
    }

    public async Task<int> CountAsync(AttendanceFilter? filter = null, CancellationToken ct = default)
    {
        var query = _dbSet.AsQueryable();

        if (filter?.InternId.HasValue == true)
            query = query.Where(a => a.InternId == filter.InternId.Value);
        if (!string.IsNullOrWhiteSpace(filter?.Status))
            query = query.Where(a => a.Status == filter.Status);
        if (filter?.FromDate.HasValue == true)
            query = query.Where(a => a.AttendanceDate >= filter.FromDate.Value);
        if (filter?.ToDate.HasValue == true)
            query = query.Where(a => a.AttendanceDate <= filter.ToDate.Value);

        return await query.CountAsync(ct);
    }

    public async Task<string?> GetUserNameAsync(int userId, CancellationToken ct = default)
    {
        return await _context.Users.Where(u => u.UserId == userId).Select(u => u.FullName).FirstOrDefaultAsync(ct);
    }
}

public class EfDepartmentRepository : GenericRepository<Department>, IDepartmentRepository
{
    public EfDepartmentRepository(AppDbContext context) : base(context) { }

    public async Task<DepartmentDetailDto?> GetDetailByIdAsync(int id, CancellationToken ct = default)
    {
        var dept = await _dbSet.FirstOrDefaultAsync(d => d.DepartmentId == id, ct);
        if (dept is null) return null;

        var headUserName = dept.HeadUserId.HasValue
            ? await _context.Users.Where(u => u.UserId == dept.HeadUserId).Select(u => u.FullName).FirstOrDefaultAsync(ct)
            : null;

        return dept.ToDetailDto(headUserName);
    }

    public async Task<IEnumerable<DepartmentDto>> GetAllDtoAsync(PaginationRequest pagination, DepartmentFilter? filter = null, CancellationToken ct = default)
    {
        var query = _dbSet.AsQueryable();

        if (!string.IsNullOrWhiteSpace(filter?.Search))
            query = query.Where(d => d.DepartmentName.Contains(filter.Search) || (d.Description != null && d.Description.Contains(filter.Search)));
        if (!string.IsNullOrWhiteSpace(filter?.Status))
            query = query.Where(d => d.Status == filter.Status);

        return await query
            .OrderByDescending(d => d.CreatedAt)
            .Skip((pagination.Page - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .Select(d => new DepartmentDto(
                d.DepartmentId, d.DepartmentName, d.Description, d.HeadUserId,
                _context.Users.Where(u => u.UserId == d.HeadUserId).Select(u => u.FullName).FirstOrDefault(),
                0, 0,
                d.Status, d.CreatedAt, d.UpdatedAt))
            .ToListAsync(ct);
    }

    public async Task<int> CountAsync(DepartmentFilter? filter = null, CancellationToken ct = default)
    {
        var query = _dbSet.AsQueryable();

        if (!string.IsNullOrWhiteSpace(filter?.Search))
            query = query.Where(d => d.DepartmentName.Contains(filter.Search) || (d.Description != null && d.Description.Contains(filter.Search)));
        if (!string.IsNullOrWhiteSpace(filter?.Status))
            query = query.Where(d => d.Status == filter.Status);

        return await query.CountAsync(ct);
    }
}
