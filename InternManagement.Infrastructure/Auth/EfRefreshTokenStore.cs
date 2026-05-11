using InternManagement.Application.Auth;
using InternManagement.Domain.Entities;
using InternManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace InternManagement.Infrastructure.Auth;

public class EfRefreshTokenStore(AppDbContext db) : IRefreshTokenStore
{
    public async Task AddAsync(RefreshToken token, CancellationToken ct)
    {
        db.RefreshTokens.Add(token);
        await db.SaveChangesAsync(ct);
    }

    public Task<RefreshToken?> FindActiveAsync(string token, CancellationToken ct)
    {
        var now = DateTime.UtcNow;
        return db.RefreshTokens
            .FirstOrDefaultAsync(x => x.Token == token && x.RevokedAt == null && x.ExpiresAt > now, ct);
    }

    public Task<RefreshToken?> FindByTokenAsync(string token, CancellationToken ct)
    {
        return db.RefreshTokens.FirstOrDefaultAsync(x => x.Token == token, ct);
    }

    public async Task RevokeAsync(RefreshToken token, CancellationToken ct)
    {
        token.RevokedAt = DateTime.UtcNow;
        db.RefreshTokens.Update(token);
        await db.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(RefreshToken token, CancellationToken ct)
    {
        db.RefreshTokens.Update(token);
        await db.SaveChangesAsync(ct);
    }
}

