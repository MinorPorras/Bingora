using Bingora.Domain.Entities;

namespace Bingora.Application.Interfaces;

public interface IGameRepository
{
    Task AddAsync(Game game, CancellationToken ct = default);

    Task<Game?> GetByIdWithDetailsAsync(Guid id, Guid ownerId, CancellationToken ct = default);
}