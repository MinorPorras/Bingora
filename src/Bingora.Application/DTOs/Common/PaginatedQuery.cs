namespace Bingora.Application.DTOs.Common;

public record PaginatedQuery(int PageNumber = 1, int PageSize = 10);