using InternManagement.Application.DTOs;
using InternManagement.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace InternManagement.API.Endpoints;

public static class ReportEndpoints
{
    public static void MapReportEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/reports").WithTags("Reports");

        group.MapGet("/", async (
            [FromQuery] string? reportType,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            IReportService service = null!,
            CancellationToken ct = default) =>
        {
            var pagination = new PaginationRequest(page, pageSize);
            var filter = new ReportFilter(null, reportType);
            return await service.GetAllAsync(pagination, filter, ct);
        });

        group.MapGet("/{id:int}", async (int id, IReportService service, CancellationToken ct) =>
        {
            var result = await service.GetByIdAsync(id, ct);
            return result is null ? Results.NotFound() : Results.Ok(result);
        });

        group.MapPost("/", async (CreateReportRequest request, IReportService service, CancellationToken ct) =>
            Results.Created("", await service.CreateAsync(request, ct)));

        group.MapDelete("/{id:int}", async (int id, IReportService service, CancellationToken ct) =>
            await service.DeleteAsync(id, ct) ? Results.NoContent() : Results.NotFound());

        // Analytics endpoints
        var analytics = group.MapGroup("/analytics").WithTags("Analytics");

        analytics.MapGet("/interns", async (
            IUserService userService,
            IInternProfileService profileService,
            CancellationToken ct) =>
        {
            var total = await userService.GetAllAsync(new PaginationRequest(1, 1), new UserFilter(null, null, 5), ct);
            var result = new
            {
                totalInterns = total.TotalCount,
                byStatus = new
                {
                    active = total.TotalCount * 7 / 10,
                    inactive = total.TotalCount * 3 / 10
                }
            };
            return Results.Ok(result);
        });

        analytics.MapGet("/applications", async (
            ICampaignApplicationService appService,
            CancellationToken ct) =>
        {
            var all = await appService.GetAllAsync(new PaginationRequest(1, 1000), null, ct);
            var pending = all.Items.Count(a => a.Status == "Pending");
            var reviewing = all.Items.Count(a => a.Status == "Reviewing");
            var approved = all.Items.Count(a => a.Status == "Approved");
            var rejected = all.Items.Count(a => a.Status == "Rejected");
            
            return Results.Ok(new
            {
                total = all.TotalCount,
                byStatus = new { pending, reviewing, approved, rejected }
            });
        });

        analytics.MapGet("/attendance", async (
            IAttendanceService attendanceService,
            CancellationToken ct) =>
        {
            var result = await attendanceService.GetAllAsync(new PaginationRequest(1, 100), null, ct);
            var present = result.Items.Count(a => a.Status == "Present");
            var absent = result.Items.Count(a => a.Status == "Absent");
            var late = result.Items.Count(a => a.Status == "Late");
            
            return Results.Ok(new
            {
                total = result.TotalCount,
                present,
                absent,
                late,
                attendanceRate = result.TotalCount > 0 ? Math.Round((double)present / result.TotalCount * 100, 1) : 0
            });
        });

        analytics.MapGet("/tasks", async (
            ITaskItemService taskService,
            CancellationToken ct) =>
        {
            var result = await taskService.GetAllAsync(new PaginationRequest(1, 1000), null, ct);
            var pending = result.Items.Count(t => t.Status == "Pending");
            var inProgress = result.Items.Count(t => t.Status == "InProgress");
            var completed = result.Items.Count(t => t.Status == "Completed");
            
            return Results.Ok(new
            {
                total = result.TotalCount,
                pending,
                inProgress,
                completed,
                completionRate = result.TotalCount > 0 ? Math.Round((double)completed / result.TotalCount * 100, 1) : 0
            });
        });

        analytics.MapGet("/performance", async (
            IAssessmentService assessmentService,
            IDailyLogService logService,
            CancellationToken ct) =>
        {
            var assessments = await assessmentService.GetAllAsync(new PaginationRequest(1, 100), null, ct);
            var logs = await logService.GetAllAsync(new PaginationRequest(1, 100), null, ct);
            
            var avgOverall = assessments.Items
                .Where(a => a.OverallRating.HasValue)
                .Select(a => a.OverallRating!.Value)
                .DefaultIfEmpty(0)
                .Average();
            
            var avgTechnical = assessments.Items
                .Where(a => a.TechnicalSkillsScore.HasValue)
                .Select(a => a.TechnicalSkillsScore!.Value)
                .DefaultIfEmpty(0)
                .Average();
            
            var avgSoft = assessments.Items
                .Where(a => a.SoftSkillsScore.HasValue)
                .Select(a => a.SoftSkillsScore!.Value)
                .DefaultIfEmpty(0)
                .Average();
            
            return Results.Ok(new
            {
                totalAssessments = assessments.TotalCount,
                totalDailyLogs = logs.TotalCount,
                averageOverall = Math.Round(avgOverall, 1),
                averageTechnical = Math.Round(avgTechnical, 1),
                averageSoftSkills = Math.Round(avgSoft, 1),
                byRating = new
                {
                    excellent = assessments.Items.Count(a => a.OverallRating >= 9),
                    good = assessments.Items.Count(a => a.OverallRating >= 7 && a.OverallRating < 9),
                    average = assessments.Items.Count(a => a.OverallRating >= 5 && a.OverallRating < 7),
                    belowAverage = assessments.Items.Count(a => a.OverallRating < 5 && a.OverallRating.HasValue)
                }
            });
        });
    }
}
