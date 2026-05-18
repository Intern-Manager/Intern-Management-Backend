namespace InternManagement.Application.Auth;

using System.Text.Json.Serialization;

public record LoginRequest(
    [property: JsonPropertyName("email")] string Email,
    [property: JsonPropertyName("password")] string Password);

public record RegisterRequest(
    [property: JsonPropertyName("fullName")] string FullName,
    [property: JsonPropertyName("email")] string Email,
    [property: JsonPropertyName("password")] string Password,
    [property: JsonPropertyName("roleId")] int RoleId = 1,
    [property: JsonPropertyName("phone")] string? Phone = null);

public record AuthTokens(
    [property: JsonPropertyName("accessToken")] string AccessToken,
    [property: JsonPropertyName("refreshToken")] string RefreshToken,
    [property: JsonPropertyName("accessTokenExpiresAt")] DateTime AccessTokenExpiresAt,
    [property: JsonPropertyName("refreshTokenExpiresAt")] DateTime RefreshTokenExpiresAt);

public record LoginResponse(
    [property: JsonPropertyName("userId")] int UserId,
    [property: JsonPropertyName("fullName")] string FullName,
    [property: JsonPropertyName("email")] string Email,
    [property: JsonPropertyName("roleId")] int RoleId,
    [property: JsonPropertyName("status")] string Status,
    [property: JsonPropertyName("emailVerified")] bool EmailVerified,
    [property: JsonPropertyName("tokens")] AuthTokens Tokens);

public record RegisterResponse(
    [property: JsonPropertyName("userId")] int UserId,
    [property: JsonPropertyName("fullName")] string FullName,
    [property: JsonPropertyName("email")] string Email,
    [property: JsonPropertyName("roleId")] int RoleId,
    [property: JsonPropertyName("status")] string Status);

public record LogoutRequest([property: JsonPropertyName("refreshToken")] string RefreshToken);

public record RefreshTokenRequest([property: JsonPropertyName("refreshToken")] string RefreshToken);

