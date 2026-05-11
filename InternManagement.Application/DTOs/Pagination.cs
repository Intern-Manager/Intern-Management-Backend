using InternManagement.Application.Repositories;

namespace InternManagement.Application.DTOs;

public record PaginationRequest(int Page = 1, int PageSize = 20);

public record PaginatedResult<T>(
    IEnumerable<T> Items,
    int TotalCount,
    int Page,
    int PageSize,
    int TotalPages);

public static class PaginationExtensions
{
    public static PaginatedResult<T> ToPaginatedResult<T>(this IEnumerable<T> query, PaginationRequest pagination, int totalCount)
    {
        var totalPages = (int)Math.Ceiling(totalCount / (double)pagination.PageSize);
        return new PaginatedResult<T>(
            query.Skip((pagination.Page - 1) * pagination.PageSize).Take(pagination.PageSize),
            totalCount,
            pagination.Page,
            pagination.PageSize,
            totalPages);
    }
}
