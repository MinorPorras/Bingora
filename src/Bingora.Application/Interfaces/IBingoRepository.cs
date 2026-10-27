using Bingora.Application.DTOs.Bingo;
using Bingora.Domain.Common;
using Bingora.Domain.Entities;

namespace Bingora.Application.Interfaces;

public interface IBingoRepository
{
    Task<PaginatedResult<Bingo>> GetPagedAsync(Guid ownerId, int pageNumber, int pageSize,
        CancellationToken ct = default);

    public Task<PaginatedResult<Bingo>> GetPagedAsyncIgnoringFilters(Guid ownerId, int pageNumber, int pageSize,
        CancellationToken ct = default);

    Task<Bingo?> GetByIdAsync(Guid id, Guid ownerId, CancellationToken ct = default);
    Task<Bingo?> GetByIdAsyncIgnoringFilters(Guid id, Guid ownerId, CancellationToken ct = default);

    Task<PaginatedResult<Bingo>> GetPagedByNameAsync(Guid ownerId, string name, int pageNumber, int pageSize,
        CancellationToken cancellationToken = default);

    Task AddAsync(Bingo bingo, CancellationToken ct = default);

    Task UpdateAsync(Bingo bingo, CancellationToken ct = default);
}