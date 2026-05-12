using System.Linq.Expressions;
using InternManagement.Application.DTOs;
using InternManagement.Domain.Entities;
using InternManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace InternManagement.Infrastructure.Repositories;

/// <summary>
/// Generic base repository với CRUD operations và pagination
/// Giảm boilerplate code cho tất cả entities
/// </summary>
public abstract class BaseRepository<TEntity, TKey> : IBaseRepository<TEntity, TKey>
    where TEntity : class
{
    protected readonly AppDbContext _context;
    protected readonly DbSet<TEntity> _dbSet;

    protected BaseRepository(AppDbContext context)
    {
        _context = context;
        _dbSet = context.Set<TEntity>();
    }

    // === CRUD Operations ===

    public virtual async Task<TEntity?> GetByIdAsync(TKey id, CancellationToken ct = default)
        => await _dbSet.FindAsync([id], ct);

    public virtual async Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken ct = default)
        => await _dbSet.ToListAsync(ct);

    public virtual async Task<IEnumerable<TEntity>> FindAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken ct = default)
        => await _dbSet.Where(predicate).ToListAsync(ct);

    public virtual async Task<int> CountAsync(
        Expression<Func<TEntity, bool>>? predicate = null,
        CancellationToken ct = default)
        => predicate is null
            ? await _dbSet.CountAsync(ct)
            : await _dbSet.CountAsync(predicate, ct);

    public virtual async Task AddAsync(TEntity entity, CancellationToken ct = default)
    {
        await _dbSet.AddAsync(entity, ct);
        await _context.SaveChangesAsync(ct);
    }

    public virtual async Task UpdateAsync(TEntity entity, CancellationToken ct = default)
    {
        _dbSet.Update(entity);
        await _context.SaveChangesAsync(ct);
    }

    public virtual async Task DeleteAsync(TKey id, CancellationToken ct = default)
    {
        var entity = await GetByIdAsync(id, ct);
        if (entity is not null)
        {
            _dbSet.Remove(entity);
            await _context.SaveChangesAsync(ct);
        }
    }

    public virtual async Task<bool> ExistsAsync(TKey id, CancellationToken ct = default)
    {
        var entity = await GetByIdAsync(id, ct);
        return entity is not null;
    }

    // === Pagination Helpers ===

    protected IQueryable<TEntity> ApplyPagination(
        IQueryable<TEntity> query,
        PaginationRequest pagination)
        => query
            .Skip((pagination.Page - 1) * pagination.PageSize)
            .Take(pagination.PageSize);

    protected async Task<(IEnumerable<TEntity> Items, int TotalCount)> GetPagedAsync(
        IQueryable<TEntity> query,
        PaginationRequest pagination,
        CancellationToken ct = default)
    {
        var totalCount = await query.CountAsync(ct);
        var items = await ApplyPagination(query, pagination).ToListAsync(ct);
        return (items, totalCount);
    }
}

/// <summary>
/// Interface cho BaseRepository
/// </summary>
public interface IBaseRepository<TEntity, in TKey> where TEntity : class
{
    Task<TEntity?> GetByIdAsync(TKey id, CancellationToken ct = default);
    Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken ct = default);
    Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken ct = default);
    Task<int> CountAsync(Expression<Func<TEntity, bool>>? predicate = null, CancellationToken ct = default);
    Task AddAsync(TEntity entity, CancellationToken ct = default);
    Task UpdateAsync(TEntity entity, CancellationToken ct = default);
    Task DeleteAsync(TKey id, CancellationToken ct = default);
    Task<bool> ExistsAsync(TKey id, CancellationToken ct = default);
}
