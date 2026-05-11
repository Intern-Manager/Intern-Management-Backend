using InternManagement.Domain.Entities;

namespace InternManagement.Application.Auth;

public class AuthService(
    IUserRepository users,
    IRoleRepository roles,
    IPasswordHasher passwordHasher,
    IJwtTokenGenerator jwtTokenGenerator,
    IRefreshTokenGenerator refreshTokenGenerator,
    IRefreshTokenStore refreshTokenStore) : IAuthService
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
            throw new InvalidOperationException("Invalid credentials.");

        var (accessToken, accessExpiresAt) = jwtTokenGenerator.CreateAccessToken(user);
        var (refreshToken, refreshExpiresAt) = refreshTokenGenerator.CreateRefreshToken();

        await refreshTokenStore.AddAsync(new RefreshToken
        {
            UserId = user.UserId,
            Token = refreshToken,
            ExpiresAt = refreshExpiresAt,
            CreatedAt = DateTime.UtcNow
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

        return new AuthTokens(accessToken, newRefreshToken, accessExpiresAt, refreshExpiresAt);
    }

    public async Task LogoutAsync(LogoutRequest request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.RefreshToken))
            return;

        var rt = await refreshTokenStore.FindActiveAsync(request.RefreshToken, ct);
        if (rt is null)
            return;

        await refreshTokenStore.RevokeAsync(rt, ct);
    }
}

