using System.Text;
using InternManagement.API.Endpoints;
using InternManagement.Application.Auth;
using InternManagement.Infrastructure.Auth;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace InternManagement.API.Extensions;

public static class PresentationExtensions
{
    public static IServiceCollection AddPresentation(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
            {
                policy
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            });
        });

        var jwt = configuration.GetSection("Jwt").Get<JwtOptions>() ?? new JwtOptions();
        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(o =>
            {
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = jwt.Issuer,
                    ValidAudience = jwt.Audience,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key))
                };
            });

        services.AddAuthorization();
        return services;
    }

    public static WebApplication MapPresentation(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.MapRoleEndpoints();
        app.MapUserEndpoints();
        app.MapInternProfileEndpoints();
        app.MapInternshipCampaignEndpoints();
        app.MapCampaignApplicationEndpoints();
        app.MapInterviewEndpoints();
        app.MapTrainingProgramEndpoints();
        app.MapLearningResourceEndpoints();
        app.MapMentorshipEndpoints();
        app.MapTaskItemEndpoints();
        app.MapDailyLogEndpoints();
        app.MapAssessmentEndpoints();
        app.MapFeedbackEndpoints();
        app.MapCommunicationEndpoints();
        app.MapNotificationEndpoints();
        app.MapReportEndpoints();
        app.MapAttendanceEndpoints();
        app.MapCertificateEndpoints();

        var auth = app.MapGroup("/auth");

        auth.MapPost("/register", async (RegisterRequest req, IAuthService svc, CancellationToken ct) =>
        {
            try
            {
                var res = await svc.RegisterAsync(req, ct);
                return Results.Ok(res);
            }
            catch (InvalidOperationException ex)
            {
                return Results.BadRequest(new { message = ex.Message });
            }
        });

        auth.MapPost("/login", async (LoginRequest req, IAuthService svc, CancellationToken ct) =>
        {
            try
            {
                var res = await svc.LoginAsync(req, ct);
                return Results.Ok(res);
            }
            catch (InvalidOperationException ex)
            {
                return Results.BadRequest(new { message = ex.Message });
            }
        });

        auth.MapPost("/refresh", async (RefreshTokenRequest req, IAuthService svc, CancellationToken ct) =>
        {
            try
            {
                var res = await svc.RefreshAsync(req, ct);
                return Results.Ok(res);
            }
            catch (InvalidOperationException ex)
            {
                return Results.BadRequest(new { message = ex.Message });
            }
        });

        auth.MapPost("/logout", async (LogoutRequest req, IAuthService svc, CancellationToken ct) =>
        {
            await svc.LogoutAsync(req, ct);
            return Results.NoContent();
        });

        return app;
    }
}
