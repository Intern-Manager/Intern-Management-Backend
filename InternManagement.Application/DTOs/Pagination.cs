using InternManagement.Application.Repositories;

namespace InternManagement.Application.DTOs;

public record PaginationRequest(int Page = 1, int PageSize = 20);

public record PaginatedResult<T>(
    List<T> Items,
    int TotalCount,
    int Page,
    int PageSize,
    int TotalPages)
{
    public bool HasPreviousPage => Page > 1;
    public bool HasNextPage => Page < TotalPages;
}

public static class PaginationExtensions
{
    public static PaginatedResult<T> ToPaginatedResult<T>(this IEnumerable<T> items, PaginationRequest pagination, int totalCount)
    {
        var totalPages = (int)Math.Ceiling(totalCount / (double)pagination.PageSize);
        return new PaginatedResult<T>(
            items.ToList(),
            totalCount,
            pagination.Page,
            pagination.PageSize,
            totalPages);
    }
}
