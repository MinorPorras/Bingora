using Bingora.Domain.Entities;

namespace Bingora.Application.Interfaces;

public interface IGameRepository
{
    Task AddAsync(Game game, CancellationToken ct = default);
    Task UpdateAsync(Game game, CancellationToken ct = default);
    Task<Game?> GetByIdWithDetailsAsync(Guid id, Guid ownerId, CancellationToken ct = default);
    Task<Game?> GetByIdAsync(Guid id, Guid ownerId, CancellationToken ct = default);
    Task<Game?> GetByCodeAsync(string code, CancellationToken ct = default);
    Task<Player?> GetPlayerByIdAsync(Guid gameId, Guid playerId, CancellationToken ct = default);
    Task<Player?> GetPlayerWithGameAsync(Guid gameId, Guid playerId, CancellationToken ct = default);
    Task<Game?> GetByIdForManagementAsync(Guid gameId, CancellationToken ct = default);
    Task AddCardsAsync(Guid gameId, IEnumerable<Card> cards, CancellationToken ct = default);
    Task AddPlayerAsync(Player player, CancellationToken ct = default);
}