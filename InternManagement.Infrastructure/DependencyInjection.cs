using InternManagement.Application.Auth;
using InternManagement.Application.Repositories;
using InternManagement.Infrastructure.Auth;
using InternManagement.Infrastructure.Persistence;
using InternManagement.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace InternManagement.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        // Auth interfaces - use fully qualified names to avoid ambiguity
        services.AddScoped<Application.Auth.IUserRepository, Auth.EfUserRepository>();
        services.AddScoped<Application.Auth.IRoleRepository, Auth.EfRoleRepository>();
        services.AddScoped<IPasswordHasher, Pbkdf2PasswordHasher>();
        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddScoped<IRefreshTokenGenerator, RefreshTokenGenerator>();
        services.AddScoped<IRefreshTokenStore, Auth.EfRefreshTokenStore>();

        // CRUD Repositories
        services.AddScoped<Application.Repositories.IUserRepository, Auth.EfUserRepository>();
        services.AddScoped<Application.Repositories.IRoleRepository, Auth.EfRoleRepository>();
        services.AddScoped<IInternProfileRepository, EfInternProfileRepository>();
        services.AddScoped<IInternshipCampaignRepository, EfInternshipCampaignRepository>();
        services.AddScoped<ICampaignApplicationRepository, EfCampaignApplicationRepository>();
        services.AddScoped<IInterviewRepository, EfInterviewRepository>();
        services.AddScoped<ITrainingProgramRepository, EfTrainingProgramRepository>();
        services.AddScoped<ILearningResourceRepository, EfLearningResourceRepository>();
        services.AddScoped<IMentorshipRepository, EfMentorshipRepository>();
        services.AddScoped<ITaskItemRepository, EfTaskItemRepository>();
        services.AddScoped<IDailyLogRepository, EfDailyLogRepository>();
        services.AddScoped<IAssessmentRepository, EfAssessmentRepository>();
        services.AddScoped<IFeedbackRepository, EfFeedbackRepository>();
        services.AddScoped<ICommunicationRepository, EfCommunicationRepository>();
        services.AddScoped<INotificationRepository, EfNotificationRepository>();
        services.AddScoped<IReportRepository, EfReportRepository>();
        services.AddScoped<IAttendanceRepository, EfAttendanceRepository>();
        services.AddScoped<ICertificateRepository, EfCertificateRepository>();

        services.Configure<JwtOptions>(configuration.GetSection("Jwt"));

        return services;
    }
}
