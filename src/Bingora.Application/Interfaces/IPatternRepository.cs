using Bingora.Domain.Common;
using Bingora.Domain.Entities;

namespace Bingora.Application.Interfaces;

public interface IPatternRepository
{
    Task<Pattern?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<PaginatedResult<Pattern>> GetAllPagedAsync(int pageNumber, int pageSize, CancellationToken ct = default);
    Task<PaginatedResult<Pattern>> GetByNamePagedAsync(int pageNumber, int pageSize, string patternName, CancellationToken ct = default);
    Task AddAsync(Pattern pattern, CancellationToken ct = default);
    Task UpdateAsync(Pattern pattern, CancellationToken ct = default);
}