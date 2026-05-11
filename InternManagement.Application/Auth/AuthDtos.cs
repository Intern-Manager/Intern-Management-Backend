namespace InternManagement.Application.Auth;

public record LoginRequest(string Email, string Password);

public record RegisterRequest(
    string FullName,
    string Email,
    string Password,
    int RoleId = 1,
    string? Phone = null);

public record AuthTokens(string AccessToken, string RefreshToken, DateTime AccessTokenExpiresAt, DateTime RefreshTokenExpiresAt);

public record LoginResponse(
    int UserId,
    string FullName,
    string Email,
    int RoleId,
    string Status,
    bool EmailVerified,
    AuthTokens Tokens);

public record RegisterResponse(
    int UserId,
    string FullName,
    string Email,
    int RoleId,
    string Status);

public record LogoutRequest(string RefreshToken);

public record RefreshTokenRequest(string RefreshToken);

