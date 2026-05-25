using System.Text;
using InternManagement.API.Endpoints;
using InternManagement.API.Hubs;
using InternManagement.API.Services;
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
                    .SetIsOriginAllowed(_ => true)
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials();
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
                // Allow SignalR to receive tokens from query string
                o.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];
                        var path = context.HttpContext.Request.Path;
                        if (!string.IsNullOrEmpty(accessToken) && (
                            path.StartsWithSegments("/hubs/notifications") ||
                            path.StartsWithSegments("/hubs/chat")))
                        {
                            context.Token = accessToken;
                        }
                        return Task.CompletedTask;
                    }
                };
            });

        services.AddAuthorization();
        services.AddSignalR();

        return services;
    }

    public static WebApplication MapPresentation(this WebApplication app)
    {
        // Always enable Swagger in development, optionally in other environments
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "Intern Management API V1");
        });

        // Map SignalR hubs
        app.MapHub<NotificationHub>("/hubs/notifications");
        app.MapHub<ChatHub>("/hubs/chat");

        // Map all CRUD endpoints
        app.MapAllEndpoints();

        // Upload endpoints (no auth required for public use)
        app.MapUploadEndpoints();

        // Audit logs endpoint
        app.MapAuditLogEndpoints();

        // Auth endpoints
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

        auth.MapPost("/forgot-password", async (ForgotPasswordRequest req, IAuthService svc, CancellationToken ct) =>
        {
            await svc.ForgotPasswordAsync(req, ct);
            // Always return success to prevent email enumeration
            return Results.Ok(new { message = "If an account exists with this email, a password reset link has been sent." });
        });

        auth.MapPost("/reset-password", async (ResetPasswordRequest req, IAuthService svc, CancellationToken ct) =>
        {
            try
            {
                await svc.ResetPasswordAsync(req, ct);
                return Results.Ok(new { message = "Password has been reset successfully." });
            }
            catch (InvalidOperationException ex)
            {
                return Results.BadRequest(new { message = ex.Message });
            }
        });

        auth.MapPost("/verify-email", async (VerifyEmailRequest req, IAuthService svc, CancellationToken ct) =>
        {
            try
            {
                await svc.VerifyEmailAsync(req, ct);
                return Results.Ok(new { message = "Email verified successfully." });
            }
            catch (InvalidOperationException ex)
            {
                return Results.BadRequest(new { message = ex.Message });
            }
        });

        return app;
    }
}
