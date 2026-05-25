using InternManagement.Domain.Entities;
using InternManagement.Application.DTOs;
using InternManagement.Application.Repositories;

namespace InternManagement.Application.Auth;

public class AuthService(
    IUserRepository users,
    IRoleRepository roles,
    IInternProfileRepository internProfiles,
    IPasswordHasher passwordHasher,
    IJwtTokenGenerator jwtTokenGenerator,
    IRefreshTokenGenerator refreshTokenGenerator,
    IRefreshTokenStore refreshTokenStore,
    Services.IAuditLogService auditLog) : IAuthService
{
    public async Task<RegisterResponse> RegisterAsync(RegisterRequest request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.FullName) ||
            string.IsNullOrWhiteSpace(request.Email) ||
            string.IsNullOrWhiteSpace(request.Password))
        {
            throw new InvalidOperationException("FullName, email, password are required.");
        }

        if (request.Password.Length < 6)
            throw new InvalidOperationException("Password must be at least 6 characters.");

        var existed = await users.FindByEmailAsync(request.Email, ct);
        if (existed is not null)
            throw new InvalidOperationException("Email already exists.");

        if (!await roles.ExistsAsync(request.RoleId, ct))
            throw new InvalidOperationException("Role does not exist.");

        var user = new User
        {
            FullName = request.FullName.Trim(),
            Email = request.Email.Trim(),
            PasswordHash = passwordHasher.Hash(request.Password),
            Phone = request.Phone,
            RoleId = request.RoleId,
            Status = "Active",
            EmailVerified = false,
            CreatedAt = DateTime.UtcNow
        };

        await users.AddAsync(user, ct);

        // Auto-create InternProfile for Intern role
        await internProfiles.AddAsync(new InternProfile
        {
            UserId = user.UserId,
            CreatedAt = DateTime.UtcNow
        }, ct);

        // Audit log: User registered
        await auditLog.LogAsync(new Services.AuditLogEntry
        {
            UserId = user.UserId,
            UserName = user.FullName,
            Action = "Registered",
            EntityType = "User",
            EntityId = user.UserId,
            Description = $"New user registered: {user.Email} (RoleId: {user.RoleId})",
            LogType = "Security"
        }, ct);

        return new RegisterResponse(
            user.UserId,
            user.FullName,
            user.Email,
            user.RoleId,
            user.Status);
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
            throw new InvalidOperationException("Email/password is required.");

        var user = await users.FindByEmailAsync(request.Email, ct);

        if (user is null || !passwordHasher.Verify(request.Password, user.PasswordHash))
        {
            // Audit log: Failed login attempt
            await auditLog.LogAsync(new Services.AuditLogEntry
            {
                UserName = request.Email,
                Action = "Login Failed",
                EntityType = "User",
                Description = $"Failed login attempt for: {request.Email}",
                LogType = "Security"
            }, ct);
            throw new InvalidOperationException("Invalid credentials.");
        }

        var (accessToken, accessExpiresAt) = jwtTokenGenerator.CreateAccessToken(user);
        var (refreshToken, refreshExpiresAt) = refreshTokenGenerator.CreateRefreshToken();

        await refreshTokenStore.AddAsync(new RefreshToken
        {
            UserId = user.UserId,
            Token = refreshToken,
            ExpiresAt = refreshExpiresAt,
            CreatedAt = DateTime.UtcNow
        }, ct);

        // Audit log: Successful login
        await auditLog.LogAsync(new Services.AuditLogEntry
        {
            UserId = user.UserId,
            UserName = user.FullName,
            Action = "Logged In",
            EntityType = "User",
            EntityId = user.UserId,
            Description = $"User logged in: {user.Email}",
            LogType = "Security"
        }, ct);

        return new LoginResponse(
            user.UserId,
            user.FullName,
            user.Email,
            user.RoleId,
            user.Status,
            user.EmailVerified,
            new AuthTokens(accessToken, refreshToken, accessExpiresAt, refreshExpiresAt)
        );
    }

    public async Task<AuthTokens> RefreshAsync(RefreshTokenRequest request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.RefreshToken))
            throw new InvalidOperationException("Refresh token is required.");

        var currentToken = await refreshTokenStore.FindActiveAsync(request.RefreshToken, ct);
        if (currentToken is null)
            throw new InvalidOperationException("Invalid refresh token.");

        var user = await users.FindByIdAsync(currentToken.UserId, ct);
        if (user is null)
            throw new InvalidOperationException("User not found.");

        var (accessToken, accessExpiresAt) = jwtTokenGenerator.CreateAccessToken(user);
        var (newRefreshToken, refreshExpiresAt) = refreshTokenGenerator.CreateRefreshToken();

        currentToken.RevokedAt = DateTime.UtcNow;
        currentToken.ReplacedByToken = newRefreshToken;
        await refreshTokenStore.UpdateAsync(currentToken, ct);

        await refreshTokenStore.AddAsync(new RefreshToken
        {
            UserId = user.UserId,
            Token = newRefreshToken,
            ExpiresAt = refreshExpiresAt,
            CreatedAt = DateTime.UtcNow
        }, ct);

        // Audit log: Token refreshed
        await auditLog.LogAsync(new Services.AuditLogEntry
        {
            UserId = user.UserId,
            UserName = user.FullName,
            Action = "Token Refreshed",
            EntityType = "User",
            EntityId = user.UserId,
            Description = $"Token refreshed for: {user.Email}",
            LogType = "Security"
        }, ct);

        return new AuthTokens(accessToken, newRefreshToken, accessExpiresAt, refreshExpiresAt);
    }

    public async Task LogoutAsync(LogoutRequest request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.RefreshToken))
            return;

        var rt = await refreshTokenStore.FindActiveAsync(request.RefreshToken, ct);
        if (rt is null)
            return;

        var user = await users.FindByIdAsync(rt.UserId, ct);

        await refreshTokenStore.RevokeAsync(rt, ct);

        // Audit log: Logout
        if (user != null)
        {
            await auditLog.LogAsync(new Services.AuditLogEntry
            {
                UserId = user.UserId,
                UserName = user.FullName,
                Action = "Logged Out",
                EntityType = "User",
                EntityId = user.UserId,
                Description = $"User logged out: {user.Email}",
                LogType = "Security"
            }, ct);
        }
    }

    public async Task ForgotPasswordAsync(ForgotPasswordRequest request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.Email))
            throw new InvalidOperationException("Email is required.");

        var user = await users.FindByEmailAsync(request.Email, ct);
        // Always return success to prevent email enumeration
        // In production, you would send an email here
        if (user is null)
            return;

        // Audit log: Password reset requested
        await auditLog.LogAsync(new Services.AuditLogEntry
        {
            UserId = user.UserId,
            UserName = user.FullName,
            Action = "Password Reset Requested",
            EntityType = "User",
            EntityId = user.UserId,
            Description = $"Password reset requested for: {request.Email}",
            LogType = "Security"
        }, ct);

        // TODO: Send password reset email
        // For now, just log (in production, integrate with email service)
        Console.WriteLine($"Password reset requested for: {request.Email}");
    }

    public async Task ResetPasswordAsync(ResetPasswordRequest request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.Token) || string.IsNullOrWhiteSpace(request.NewPassword))
            throw new InvalidOperationException("Token and new password are required.");

        if (request.NewPassword.Length < 6)
            throw new InvalidOperationException("Password must be at least 6 characters.");

        // TODO: Validate token from email link
        // For now, this is a simplified implementation
        // In production, store reset tokens in database with expiry

        // Find user by token (simplified - in production use a token store)
        // This would require a PasswordResetToken entity
        throw new InvalidOperationException("Password reset is not yet fully implemented. Please contact support.");
    }

    public async Task VerifyEmailAsync(VerifyEmailRequest request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.Token))
            throw new InvalidOperationException("Verification token is required.");

        var user = await users.FindByIdAsync(0, ct); // Placeholder - would look up by token
        // In production, verify token and update EmailVerified = true
        // For now, this is a placeholder
        Console.WriteLine($"Email verification attempted with token: {request.Token}");
    }
}

