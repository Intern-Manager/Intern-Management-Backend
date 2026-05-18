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
                // Allow SignalR to receive tokens from query string
                o.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];
                        var path = context.HttpContext.Request.Path;
                        if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs/notifications"))
                        {
                            context.Token = accessToken;
                        }
                        return Task.CompletedTask;
                    }
                };
            });

        services.AddAuthorization();
        services.AddScoped<CloudinaryService>();
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

        // Map SignalR hub
        app.MapHub<NotificationHub>("/hubs/notifications");

        // Map all CRUD endpoints
        app.MapAllEndpoints();

        // Upload endpoints (no auth required for public use)
        app.MapUploadEndpoints();

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

        return app;
    }
}
