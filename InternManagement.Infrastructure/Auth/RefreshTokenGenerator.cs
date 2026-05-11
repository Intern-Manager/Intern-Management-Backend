using System.Security.Cryptography;
using InternManagement.Application.Auth;

namespace InternManagement.Infrastructure.Auth;

public class RefreshTokenGenerator : IRefreshTokenGenerator
{
    public (string token, DateTime expiresAtUtc) CreateRefreshToken()
    {
        var bytes = RandomNumberGenerator.GetBytes(64);
        var token = Convert.ToBase64String(bytes)
            .Replace("+", "-", StringComparison.Ordinal)
            .Replace("/", "_", StringComparison.Ordinal)
            .TrimEnd('=');

        return (token, DateTime.UtcNow.AddDays(14));
    }
}

