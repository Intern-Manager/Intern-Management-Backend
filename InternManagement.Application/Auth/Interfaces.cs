using InternManagement.Domain.Entities;

namespace InternManagement.Application.Auth;

public interface IUserRepository
{
    Task<User?> FindByEmailAsync(string email, CancellationToken ct);
    Task<User?> FindByIdAsync(int id, CancellationToken ct);
    Task AddAsync(User user, CancellationToken ct);
}

public interface IRoleRepository
{
    Task<bool> ExistsAsync(int roleId, CancellationToken ct);
}

public interface IPasswordHasher
{
    string Hash(string password);
    bool Verify(string password, string passwordHash);
}

public interface IJwtTokenGenerator
{
    (string token, DateTime expiresAtUtc) CreateAccessToken(User user);
}

public interface IRefreshTokenGenerator
{
    (string token, DateTime expiresAtUtc) CreateRefreshToken();
}

public interface IRefreshTokenStore
{
    Task AddAsync(RefreshToken token, CancellationToken ct);
    Task<RefreshToken?> FindActiveAsync(string token, CancellationToken ct);
    Task<RefreshToken?> FindByTokenAsync(string token, CancellationToken ct);
    Task RevokeAsync(RefreshToken token, CancellationToken ct);
    Task UpdateAsync(RefreshToken token, CancellationToken ct);
}

public interface IAuthService
{
    Task<RegisterResponse> RegisterAsync(RegisterRequest request, CancellationToken ct);
    Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken ct);
    Task<AuthTokens> RefreshAsync(RefreshTokenRequest request, CancellationToken ct);
    Task LogoutAsync(LogoutRequest request, CancellationToken ct);
    Task ForgotPasswordAsync(ForgotPasswordRequest request, CancellationToken ct);
    Task ResetPasswordAsync(ResetPasswordRequest request, CancellationToken ct);
    Task VerifyEmailAsync(VerifyEmailRequest request, CancellationToken ct);
}

