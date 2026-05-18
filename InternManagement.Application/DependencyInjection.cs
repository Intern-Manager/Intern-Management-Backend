using InternManagement.Application.Auth;
using InternManagement.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace InternManagement.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();

        // CRUD Services
        services.AddScoped<IRoleService, RoleService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IInternProfileService, InternProfileService>();
        services.AddScoped<IInternshipCampaignService, InternshipCampaignService>();
        services.AddScoped<ICampaignApplicationService, CampaignApplicationService>();
        services.AddScoped<IInterviewService, InterviewService>();
        services.AddScoped<ITrainingProgramService, TrainingProgramService>();
        services.AddScoped<ILearningResourceService, LearningResourceService>();
        services.AddScoped<IMentorshipService, MentorshipService>();
        services.AddScoped<ITaskItemService, TaskItemService>();
        services.AddScoped<ITaskSubmissionService, TaskSubmissionService>();
        services.AddScoped<IDailyLogService, DailyLogService>();
        services.AddScoped<IAssessmentService, AssessmentService>();
        services.AddScoped<IFeedbackService, FeedbackService>();
        services.AddScoped<ICommunicationService, CommunicationService>();
        services.AddScoped<INotificationService, NotificationService>();
        services.AddScoped<IReportService, ReportService>();
        services.AddScoped<IAttendanceService, AttendanceService>();
        services.AddScoped<ICertificateService, CertificateService>();
        services.AddScoped<IDepartmentService, DepartmentService>();

        // External Services
        services.AddScoped<IImageUploadService, CloudinaryService>();

        return services;
    }
}
