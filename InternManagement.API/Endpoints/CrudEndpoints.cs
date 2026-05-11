using InternManagement.Application.DTOs;
using InternManagement.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace InternManagement.API.Endpoints;

public static class RoleEndpoints
{
    public static void MapRoleEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/roles").WithTags("Roles");

        group.MapGet("/", async (IRoleService service, CancellationToken ct) =>
            await service.GetAllAsync(ct));

        group.MapGet("/{id:int}", async (int id, IRoleService service, CancellationToken ct) =>
        {
            var result = await service.GetByIdAsync(id, ct);
            return result is null ? Results.NotFound() : Results.Ok(result);
        });

        group.MapPost("/", async (CreateRoleRequest request, IRoleService service, CancellationToken ct) =>
            Results.Created("", await service.CreateAsync(request, ct)));

        group.MapPut("/{id:int}", async (int id, UpdateRoleRequest request, IRoleService service, CancellationToken ct) =>
        {
            var result = await service.UpdateAsync(id, request, ct);
            return result is null ? Results.NotFound() : Results.Ok(result);
        });

        group.MapDelete("/{id:int}", async (int id, IRoleService service, CancellationToken ct) =>
            await service.DeleteAsync(id, ct) ? Results.NoContent() : Results.NotFound());
    }
}

public static class UserEndpoints
{
    public static void MapUserEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/users").WithTags("Users");

        group.MapGet("/", async ([FromQuery] int page, [FromQuery] int pageSize, [FromQuery] string? search,
            [FromQuery] string? status, [FromQuery] int? roleId, IUserService service, CancellationToken ct) =>
        {
            var pagination = new PaginationRequest(page, pageSize);
            var filter = new UserFilter(search, status, roleId);
            return await service.GetAllAsync(pagination, filter, ct);
        });

        group.MapGet("/{id:int}", async (int id, IUserService service, CancellationToken ct) =>
        {
            var result = await service.GetByIdAsync(id, ct);
            return result is null ? Results.NotFound() : Results.Ok(result);
        });

        group.MapPut("/{id:int}", async (int id, UpdateUserRequest request, IUserService service, CancellationToken ct) =>
        {
            var result = await service.UpdateAsync(id, request, ct);
            return result is null ? Results.NotFound() : Results.Ok(result);
        });

        group.MapDelete("/{id:int}", async (int id, IUserService service, CancellationToken ct) =>
            await service.DeleteAsync(id, ct) ? Results.NoContent() : Results.NotFound());
    }
}

public static class InternProfileEndpoints
{
    public static void MapInternProfileEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/intern-profiles").WithTags("InternProfiles");

        group.MapGet("/", async ([FromQuery] int page, [FromQuery] int pageSize,
            [FromQuery] string? search, [FromQuery] string? university, [FromQuery] string? major,
            [FromQuery] int? graduationYear, IInternProfileService service, CancellationToken ct) =>
        {
            var pagination = new PaginationRequest(page, pageSize);
            var filter = new InternProfileFilter(search, university, major, graduationYear);
            return await service.GetAllAsync(pagination, filter, ct);
        });

        group.MapGet("/{id:int}", async (int id, IInternProfileService service, CancellationToken ct) =>
        {
            var result = await service.GetByIdAsync(id, ct);
            return result is null ? Results.NotFound() : Results.Ok(result);
        });

        group.MapPost("/", async (CreateInternProfileRequest request, IInternProfileService service, CancellationToken ct) =>
            Results.Created("", await service.CreateAsync(0, request, ct)));

        group.MapPut("/{id:int}", async (int id, UpdateInternProfileRequest request, IInternProfileService service, CancellationToken ct) =>
        {
            var result = await service.UpdateAsync(id, request, ct);
            return result is null ? Results.NotFound() : Results.Ok(result);
        });

        group.MapDelete("/{id:int}", async (int id, IInternProfileService service, CancellationToken ct) =>
            await service.DeleteAsync(id, ct) ? Results.NoContent() : Results.NotFound());
    }
}

public static class InternshipCampaignEndpoints
{
    public static void MapInternshipCampaignEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/campaigns").WithTags("InternshipCampaigns");

        group.MapGet("/", async ([FromQuery] int page, [FromQuery] int pageSize,
            [FromQuery] string? search, [FromQuery] string? status, [FromQuery] string? department,
            [FromQuery] string? location, IInternshipCampaignService service, CancellationToken ct) =>
        {
            var pagination = new PaginationRequest(page, pageSize);
            var filter = new InternshipCampaignFilter(search, status, department, location);
            return await service.GetAllAsync(pagination, filter, ct);
        });

        group.MapGet("/{id:int}", async (int id, IInternshipCampaignService service, CancellationToken ct) =>
        {
            var result = await service.GetByIdAsync(id, ct);
            return result is null ? Results.NotFound() : Results.Ok(result);
        });

        group.MapPost("/", async (CreateInternshipCampaignRequest request, IInternshipCampaignService service, CancellationToken ct) =>
            Results.Created("", await service.CreateAsync(request, ct)));

        group.MapPut("/{id:int}", async (int id, UpdateInternshipCampaignRequest request, IInternshipCampaignService service, CancellationToken ct) =>
        {
            var result = await service.UpdateAsync(id, request, ct);
            return result is null ? Results.NotFound() : Results.Ok(result);
        });

        group.MapDelete("/{id:int}", async (int id, IInternshipCampaignService service, CancellationToken ct) =>
            await service.DeleteAsync(id, ct) ? Results.NoContent() : Results.NotFound());
    }
}

public static class CampaignApplicationEndpoints
{
    public static void MapCampaignApplicationEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/applications").WithTags("CampaignApplications");

        group.MapGet("/", async ([FromQuery] int page, [FromQuery] int pageSize,
            [FromQuery] string? search, [FromQuery] string? status, [FromQuery] int? campaignId,
            ICampaignApplicationService service, CancellationToken ct) =>
        {
            var pagination = new PaginationRequest(page, pageSize);
            var filter = new CampaignApplicationFilter(search, status, campaignId);
            return await service.GetAllAsync(pagination, filter, ct);
        });

        group.MapGet("/{id:int}", async (int id, ICampaignApplicationService service, CancellationToken ct) =>
        {
            var result = await service.GetByIdAsync(id, ct);
            return result is null ? Results.NotFound() : Results.Ok(result);
        });

        group.MapPost("/", async (CreateCampaignApplicationRequest request, ICampaignApplicationService service, CancellationToken ct) =>
            Results.Created("", await service.CreateAsync(request, ct)));

        group.MapPut("/{id:int}", async (int id, UpdateCampaignApplicationRequest request, ICampaignApplicationService service, CancellationToken ct) =>
        {
            var result = await service.UpdateAsync(id, request, ct);
            return result is null ? Results.NotFound() : Results.Ok(result);
        });

        group.MapDelete("/{id:int}", async (int id, ICampaignApplicationService service, CancellationToken ct) =>
            await service.DeleteAsync(id, ct) ? Results.NoContent() : Results.NotFound());
    }
}

public static class InterviewEndpoints
{
    public static void MapInterviewEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/interviews").WithTags("Interviews");

        group.MapGet("/", async ([FromQuery] int page, [FromQuery] int pageSize,
            [FromQuery] string? status, [FromQuery] int? campaignId, [FromQuery] int? internId,
            [FromQuery] int? interviewerId, [FromQuery] DateTime? fromDate, [FromQuery] DateTime? toDate,
            IInterviewService service, CancellationToken ct) =>
        {
            var pagination = new PaginationRequest(page, pageSize);
            var filter = new InterviewFilter(null, status, campaignId, internId, interviewerId, fromDate, toDate);
            return await service.GetAllAsync(pagination, filter, ct);
        });

        group.MapGet("/{id:int}", async (int id, IInterviewService service, CancellationToken ct) =>
        {
            var result = await service.GetByIdAsync(id, ct);
            return result is null ? Results.NotFound() : Results.Ok(result);
        });

        group.MapPost("/", async (CreateInterviewRequest request, IInterviewService service, CancellationToken ct) =>
            Results.Created("", await service.CreateAsync(request, ct)));

        group.MapPut("/{id:int}", async (int id, UpdateInterviewRequest request, IInterviewService service, CancellationToken ct) =>
        {
            var result = await service.UpdateAsync(id, request, ct);
            return result is null ? Results.NotFound() : Results.Ok(result);
        });

        group.MapDelete("/{id:int}", async (int id, IInterviewService service, CancellationToken ct) =>
            await service.DeleteAsync(id, ct) ? Results.NoContent() : Results.NotFound());
    }
}

public static class TrainingProgramEndpoints
{
    public static void MapTrainingProgramEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/training-programs").WithTags("TrainingPrograms");

        group.MapGet("/", async ([FromQuery] int page, [FromQuery] int pageSize,
            [FromQuery] string? search, [FromQuery] string? status, [FromQuery] int? coordinatorId,
            ITrainingProgramService service, CancellationToken ct) =>
        {
            var pagination = new PaginationRequest(page, pageSize);
            var filter = new TrainingProgramFilter(search, status, coordinatorId);
            return await service.GetAllAsync(pagination, filter, ct);
        });

        group.MapGet("/{id:int}", async (int id, ITrainingProgramService service, CancellationToken ct) =>
        {
            var result = await service.GetByIdAsync(id, ct);
            return result is null ? Results.NotFound() : Results.Ok(result);
        });

        group.MapPost("/", async (CreateTrainingProgramRequest request, ITrainingProgramService service, CancellationToken ct) =>
            Results.Created("", await service.CreateAsync(request, ct)));

        group.MapPut("/{id:int}", async (int id, UpdateTrainingProgramRequest request, ITrainingProgramService service, CancellationToken ct) =>
        {
            var result = await service.UpdateAsync(id, request, ct);
            return result is null ? Results.NotFound() : Results.Ok(result);
        });

        group.MapDelete("/{id:int}", async (int id, ITrainingProgramService service, CancellationToken ct) =>
            await service.DeleteAsync(id, ct) ? Results.NoContent() : Results.NotFound());
    }
}

public static class LearningResourceEndpoints
{
    public static void MapLearningResourceEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/learning-resources").WithTags("LearningResources");

        group.MapGet("/", async ([FromQuery] int page, [FromQuery] int pageSize,
            [FromQuery] string? search, [FromQuery] string? resourceType, [FromQuery] int? programId,
            ILearningResourceService service, CancellationToken ct) =>
        {
            var pagination = new PaginationRequest(page, pageSize);
            var filter = new LearningResourceFilter(search, resourceType, programId);
            return await service.GetAllAsync(pagination, filter, ct);
        });

        group.MapGet("/{id:int}", async (int id, ILearningResourceService service, CancellationToken ct) =>
        {
            var result = await service.GetByIdAsync(id, ct);
            return result is null ? Results.NotFound() : Results.Ok(result);
        });

        group.MapPost("/", async (CreateLearningResourceRequest request, ILearningResourceService service, CancellationToken ct) =>
            Results.Created("", await service.CreateAsync(request, ct)));

        group.MapPut("/{id:int}", async (int id, UpdateLearningResourceRequest request, ILearningResourceService service, CancellationToken ct) =>
        {
            var result = await service.UpdateAsync(id, request, ct);
            return result is null ? Results.NotFound() : Results.Ok(result);
        });

        group.MapDelete("/{id:int}", async (int id, ILearningResourceService service, CancellationToken ct) =>
            await service.DeleteAsync(id, ct) ? Results.NoContent() : Results.NotFound());
    }
}

public static class MentorshipEndpoints
{
    public static void MapMentorshipEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/mentorships").WithTags("Mentorships");

        group.MapGet("/", async ([FromQuery] int page, [FromQuery] int pageSize,
            [FromQuery] string? status, [FromQuery] int? mentorId, [FromQuery] int? internId,
            [FromQuery] int? programId, IMentorshipService service, CancellationToken ct) =>
        {
            var pagination = new PaginationRequest(page, pageSize);
            var filter = new MentorshipFilter(status, mentorId, internId, programId);
            return await service.GetAllAsync(pagination, filter, ct);
        });

        group.MapGet("/{id:int}", async (int id, IMentorshipService service, CancellationToken ct) =>
        {
            var result = await service.GetByIdAsync(id, ct);
            return result is null ? Results.NotFound() : Results.Ok(result);
        });

        group.MapPost("/", async (CreateMentorshipRequest request, IMentorshipService service, CancellationToken ct) =>
            Results.Created("", await service.CreateAsync(request, ct)));

        group.MapPut("/{id:int}", async (int id, UpdateMentorshipRequest request, IMentorshipService service, CancellationToken ct) =>
        {
            var result = await service.UpdateAsync(id, request, ct);
            return result is null ? Results.NotFound() : Results.Ok(result);
        });

        group.MapDelete("/{id:int}", async (int id, IMentorshipService service, CancellationToken ct) =>
            await service.DeleteAsync(id, ct) ? Results.NoContent() : Results.NotFound());
    }
}

public static class TaskItemEndpoints
{
    public static void MapTaskItemEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/tasks").WithTags("Tasks");

        group.MapGet("/", async ([FromQuery] int page, [FromQuery] int pageSize,
            [FromQuery] string? search, [FromQuery] string? status, [FromQuery] string? priority,
            [FromQuery] int? internId, [FromQuery] int? assignedBy, [FromQuery] int? programId,
            ITaskItemService service, CancellationToken ct) =>
        {
            var pagination = new PaginationRequest(page, pageSize);
            var filter = new TaskItemFilter(search, status, priority, internId, assignedBy, programId);
            return await service.GetAllAsync(pagination, filter, ct);
        });

        group.MapGet("/{id:int}", async (int id, ITaskItemService service, CancellationToken ct) =>
        {
            var result = await service.GetByIdAsync(id, ct);
            return result is null ? Results.NotFound() : Results.Ok(result);
        });

        group.MapPost("/", async (CreateTaskItemRequest request, ITaskItemService service, CancellationToken ct) =>
            Results.Created("", await service.CreateAsync(request, ct)));

        group.MapPut("/{id:int}", async (int id, UpdateTaskItemRequest request, ITaskItemService service, CancellationToken ct) =>
        {
            var result = await service.UpdateAsync(id, request, ct);
            return result is null ? Results.NotFound() : Results.Ok(result);
        });

        group.MapDelete("/{id:int}", async (int id, ITaskItemService service, CancellationToken ct) =>
            await service.DeleteAsync(id, ct) ? Results.NoContent() : Results.NotFound());
    }
}

public static class DailyLogEndpoints
{
    public static void MapDailyLogEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/daily-logs").WithTags("DailyLogs");

        group.MapGet("/", async ([FromQuery] int page, [FromQuery] int pageSize,
            [FromQuery] string? search, [FromQuery] int? internId, [FromQuery] int? mentorId,
            [FromQuery] DateOnly? fromDate, [FromQuery] DateOnly? toDate,
            IDailyLogService service, CancellationToken ct) =>
        {
            var pagination = new PaginationRequest(page, pageSize);
            var filter = new DailyLogFilter(search, internId, mentorId, fromDate, toDate);
            return await service.GetAllAsync(pagination, filter, ct);
        });

        group.MapGet("/{id:int}", async (int id, IDailyLogService service, CancellationToken ct) =>
        {
            var result = await service.GetByIdAsync(id, ct);
            return result is null ? Results.NotFound() : Results.Ok(result);
        });

        group.MapPost("/", async (CreateDailyLogRequest request, IDailyLogService service, CancellationToken ct) =>
            Results.Created("", await service.CreateAsync(request, ct)));

        group.MapPut("/{id:int}", async (int id, UpdateDailyLogRequest request, IDailyLogService service, CancellationToken ct) =>
        {
            var result = await service.UpdateAsync(id, request, ct);
            return result is null ? Results.NotFound() : Results.Ok(result);
        });

        group.MapDelete("/{id:int}", async (int id, IDailyLogService service, CancellationToken ct) =>
            await service.DeleteAsync(id, ct) ? Results.NoContent() : Results.NotFound());
    }
}

public static class AssessmentEndpoints
{
    public static void MapAssessmentEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/assessments").WithTags("Assessments");

        group.MapGet("/", async ([FromQuery] int page, [FromQuery] int pageSize,
            [FromQuery] string? assessmentType, [FromQuery] int? internId, [FromQuery] int? mentorId,
            [FromQuery] int? programId, [FromQuery] DateOnly? fromDate, [FromQuery] DateOnly? toDate,
            IAssessmentService service, CancellationToken ct) =>
        {
            var pagination = new PaginationRequest(page, pageSize);
            var filter = new AssessmentFilter(assessmentType, internId, mentorId, programId, fromDate, toDate);
            return await service.GetAllAsync(pagination, filter, ct);
        });

        group.MapGet("/{id:int}", async (int id, IAssessmentService service, CancellationToken ct) =>
        {
            var result = await service.GetByIdAsync(id, ct);
            return result is null ? Results.NotFound() : Results.Ok(result);
        });

        group.MapPost("/", async (CreateAssessmentRequest request, IAssessmentService service, CancellationToken ct) =>
            Results.Created("", await service.CreateAsync(request, ct)));

        group.MapPut("/{id:int}", async (int id, UpdateAssessmentRequest request, IAssessmentService service, CancellationToken ct) =>
        {
            var result = await service.UpdateAsync(id, request, ct);
            return result is null ? Results.NotFound() : Results.Ok(result);
        });

        group.MapDelete("/{id:int}", async (int id, IAssessmentService service, CancellationToken ct) =>
            await service.DeleteAsync(id, ct) ? Results.NoContent() : Results.NotFound());
    }
}

public static class FeedbackEndpoints
{
    public static void MapFeedbackEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/feedbacks").WithTags("Feedbacks");

        group.MapGet("/", async ([FromQuery] int page, [FromQuery] int pageSize,
            [FromQuery] string? feedbackType, [FromQuery] int? internId, [FromQuery] bool? isAnonymous,
            [FromQuery] DateTime? fromDate, [FromQuery] DateTime? toDate,
            IFeedbackService service, CancellationToken ct) =>
        {
            var pagination = new PaginationRequest(page, pageSize);
            var filter = new FeedbackFilter(feedbackType, internId, isAnonymous, fromDate, toDate);
            return await service.GetAllAsync(pagination, filter, ct);
        });

        group.MapGet("/{id:int}", async (int id, IFeedbackService service, CancellationToken ct) =>
        {
            var result = await service.GetByIdAsync(id, ct);
            return result is null ? Results.NotFound() : Results.Ok(result);
        });

        group.MapPost("/", async (CreateFeedbackRequest request, IFeedbackService service, CancellationToken ct) =>
            Results.Created("", await service.CreateAsync(request, ct)));

        group.MapPut("/{id:int}", async (int id, UpdateFeedbackRequest request, IFeedbackService service, CancellationToken ct) =>
        {
            var result = await service.UpdateAsync(id, request, ct);
            return result is null ? Results.NotFound() : Results.Ok(result);
        });

        group.MapDelete("/{id:int}", async (int id, IFeedbackService service, CancellationToken ct) =>
            await service.DeleteAsync(id, ct) ? Results.NoContent() : Results.NotFound());
    }
}

public static class CommunicationEndpoints
{
    public static void MapCommunicationEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/communications").WithTags("Communications");

        group.MapGet("/", async ([FromQuery] int page, [FromQuery] int pageSize,
            [FromQuery] int? senderId, [FromQuery] int? receiverId, [FromQuery] bool? isRead,
            ICommunicationService service, CancellationToken ct) =>
        {
            var pagination = new PaginationRequest(page, pageSize);
            var filter = new CommunicationFilter(senderId, receiverId, isRead);
            return await service.GetAllAsync(pagination, filter, ct);
        });

        group.MapGet("/{id:int}", async (int id, ICommunicationService service, CancellationToken ct) =>
        {
            var result = await service.GetByIdAsync(id, ct);
            return result is null ? Results.NotFound() : Results.Ok(result);
        });

        group.MapPost("/", async (CreateCommunicationRequest request, ICommunicationService service, CancellationToken ct) =>
            Results.Created("", await service.CreateAsync(request, ct)));

        group.MapPut("/{id:int}", async (int id, UpdateCommunicationRequest request, ICommunicationService service, CancellationToken ct) =>
        {
            var result = await service.UpdateAsync(id, request, ct);
            return result is null ? Results.NotFound() : Results.Ok(result);
        });

        group.MapDelete("/{id:int}", async (int id, ICommunicationService service, CancellationToken ct) =>
            await service.DeleteAsync(id, ct) ? Results.NoContent() : Results.NotFound());
    }
}

public static class NotificationEndpoints
{
    public static void MapNotificationEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/notifications").WithTags("Notifications");

        group.MapGet("/", async ([FromQuery] int page, [FromQuery] int pageSize,
            [FromQuery] string? category, [FromQuery] int? userId, [FromQuery] bool? isRead,
            INotificationService service, CancellationToken ct) =>
        {
            var pagination = new PaginationRequest(page, pageSize);
            var filter = new NotificationFilter(category, userId, isRead);
            return await service.GetAllAsync(pagination, filter, ct);
        });

        group.MapGet("/{id:int}", async (int id, INotificationService service, CancellationToken ct) =>
        {
            var result = await service.GetByIdAsync(id, ct);
            return result is null ? Results.NotFound() : Results.Ok(result);
        });

        group.MapPost("/", async (CreateNotificationRequest request, INotificationService service, CancellationToken ct) =>
            Results.Created("", await service.CreateAsync(request, ct)));

        group.MapPut("/{id:int}", async (int id, UpdateNotificationRequest request, INotificationService service, CancellationToken ct) =>
        {
            var result = await service.UpdateAsync(id, request, ct);
            return result is null ? Results.NotFound() : Results.Ok(result);
        });

        group.MapDelete("/{id:int}", async (int id, INotificationService service, CancellationToken ct) =>
            await service.DeleteAsync(id, ct) ? Results.NoContent() : Results.NotFound());
    }
}

public static class ReportEndpoints
{
    public static void MapReportEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/reports").WithTags("Reports");

        group.MapGet("/", async ([FromQuery] int page, [FromQuery] int pageSize,
            [FromQuery] string? search, [FromQuery] string? reportType,
            IReportService service, CancellationToken ct) =>
        {
            var pagination = new PaginationRequest(page, pageSize);
            var filter = new ReportFilter(search, reportType);
            return await service.GetAllAsync(pagination, filter, ct);
        });

        group.MapGet("/{id:int}", async (int id, IReportService service, CancellationToken ct) =>
        {
            var result = await service.GetByIdAsync(id, ct);
            return result is null ? Results.NotFound() : Results.Ok(result);
        });

        group.MapPost("/", async (CreateReportRequest request, IReportService service, CancellationToken ct) =>
            Results.Created("", await service.CreateAsync(request, ct)));

        group.MapPut("/{id:int}", async (int id, UpdateReportRequest request, IReportService service, CancellationToken ct) =>
        {
            var result = await service.UpdateAsync(id, request, ct);
            return result is null ? Results.NotFound() : Results.Ok(result);
        });

        group.MapDelete("/{id:int}", async (int id, IReportService service, CancellationToken ct) =>
            await service.DeleteAsync(id, ct) ? Results.NoContent() : Results.NotFound());
    }
}

public static class AttendanceEndpoints
{
    public static void MapAttendanceEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/attendance").WithTags("Attendance");

        group.MapGet("/", async ([FromQuery] int page, [FromQuery] int pageSize,
            [FromQuery] int? internId, [FromQuery] string? status,
            [FromQuery] DateOnly? fromDate, [FromQuery] DateOnly? toDate,
            IAttendanceService service, CancellationToken ct) =>
        {
            var pagination = new PaginationRequest(page, pageSize);
            var filter = new AttendanceFilter(internId, status, fromDate, toDate);
            return await service.GetAllAsync(pagination, filter, ct);
        });

        group.MapGet("/{id:int}", async (int id, IAttendanceService service, CancellationToken ct) =>
        {
            var result = await service.GetByIdAsync(id, ct);
            return result is null ? Results.NotFound() : Results.Ok(result);
        });

        group.MapPost("/", async (CreateAttendanceRequest request, IAttendanceService service, CancellationToken ct) =>
            Results.Created("", await service.CreateAsync(request, ct)));

        group.MapPut("/{id:int}", async (int id, UpdateAttendanceRequest request, IAttendanceService service, CancellationToken ct) =>
        {
            var result = await service.UpdateAsync(id, request, ct);
            return result is null ? Results.NotFound() : Results.Ok(result);
        });

        group.MapDelete("/{id:int}", async (int id, IAttendanceService service, CancellationToken ct) =>
            await service.DeleteAsync(id, ct) ? Results.NoContent() : Results.NotFound());
    }
}

public static class CertificateEndpoints
{
    public static void MapCertificateEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/certificates").WithTags("Certificates");

        group.MapGet("/", async ([FromQuery] int page, [FromQuery] int pageSize,
            [FromQuery] string? search, [FromQuery] int? internId, [FromQuery] int? programId,
            ICertificateService service, CancellationToken ct) =>
        {
            var pagination = new PaginationRequest(page, pageSize);
            var filter = new CertificateFilter(search, internId, programId);
            return await service.GetAllAsync(pagination, filter, ct);
        });

        group.MapGet("/{id:int}", async (int id, ICertificateService service, CancellationToken ct) =>
        {
            var result = await service.GetByIdAsync(id, ct);
            return result is null ? Results.NotFound() : Results.Ok(result);
        });

        group.MapPost("/", async (CreateCertificateRequest request, ICertificateService service, CancellationToken ct) =>
            Results.Created("", await service.CreateAsync(request, ct)));

        group.MapPut("/{id:int}", async (int id, UpdateCertificateRequest request, ICertificateService service, CancellationToken ct) =>
        {
            var result = await service.UpdateAsync(id, request, ct);
            return result is null ? Results.NotFound() : Results.Ok(result);
        });

        group.MapDelete("/{id:int}", async (int id, ICertificateService service, CancellationToken ct) =>
            await service.DeleteAsync(id, ct) ? Results.NoContent() : Results.NotFound());
    }
}
