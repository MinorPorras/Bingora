using Bingora.Domain.Common;

namespace Bingora.Application.Mappings;

public static class PaginatedResultMappings
{
    public static PaginatedResult<TTarget> MapTo<T, TTarget>(
        this PaginatedResult<T> source,
        Func<T, TTarget> selector)
    {
        return new PaginatedResult<TTarget>(
            [.. source.Items.Select(selector)],
            source.CurrentPage,
            source.TotalCount,
            source.PageSize
        );
    }
}