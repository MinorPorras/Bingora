using Bingora.Application.Interfaces;
using Bingora.Domain.Entities;
using Bingora.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Bingora.Infrastructure.Repositories;

public class GameRepository(AppDbContext context) : IGameRepository
{
    public async Task AddAsync(Game game, CancellationToken ct = default)
    {
        context.Games.Add(game);
        await context.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(Game game, CancellationToken ct = default)
    {
        context.Games.Update(game);
        await context.SaveChangesAsync(ct);
    }

    public async Task<Game?> GetByIdWithDetailsAsync(Guid id, Guid ownerId, CancellationToken ct = default) =>
        await context.Games
            .AsNoTracking()
            .Where(w => w.Id == id && w.Bingo.OwnerId == ownerId)
            .Include(g => g.GamePatterns)
            .ThenInclude(gp => gp.Pattern)
            .FirstOrDefaultAsync(ct);

    public async Task<Game?> GetByIdAsync(Guid id, Guid ownerId, CancellationToken ct = default) =>
        await context.Games
            .Where(w => w.Id == id && w.Bingo.OwnerId == ownerId)
            .FirstOrDefaultAsync(ct);

    public async Task<Game?> GetByCodeAsync(string code, CancellationToken ct = default) =>
        await context.Games
            .Include(g => g.Cards)
            .Include(g => g.Players)
            .FirstOrDefaultAsync(g => g.GameCode == code, ct);

    public async Task<Player?> GetPlayerByIdAsync(Guid gameId, Guid playerId, CancellationToken ct = default) =>
        await context.Players.FirstOrDefaultAsync(p => p.Id == playerId && p.GameId == gameId, ct);

    public async Task<Player?> GetPlayerWithGameAsync(Guid gameId, Guid playerId, CancellationToken ct = default) =>
        await context.Players.Include(p => p.Game).FirstOrDefaultAsync(p => p.Id == playerId && p.GameId == gameId, ct);

    public async Task<Game?> GetByIdForManagementAsync(Guid gameId, CancellationToken ct = default) =>
        await context.Games.Include(g => g.Players)
            .Include(g => g.Cards)
            .FirstOrDefaultAsync(g => g.Id == gameId, ct);
}