namespace Bingora.Domain.Common;

public class PaginatedResult<T>
{
    public PaginatedResult(IReadOnlyList<T> items, int currentPage, int totalCount, int pageSize)
    {
        Items = items;
        CurrentPage = currentPage;
        TotalPages = (int)Math.Ceiling((double)totalCount / pageSize);
        TotalCount = totalCount;
        PageSize = pageSize;
    }

    public IReadOnlyList<T> Items { get; }
    public int CurrentPage { get; }
    public int TotalPages { get; }
    public int TotalCount { get; }
    public int PageSize { get; }

    public bool HasPreviousPage => CurrentPage > 1;
    public bool HasNextPage => CurrentPage < TotalPages;
}