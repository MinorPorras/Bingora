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

    public async Task<Game?> GetByIdWithDetailsAsync(Guid id, Guid ownerId, CancellationToken ct = default) =>
        await context.Games
            .AsNoTracking()
            .Where(w => w.Id == id && w.Bingo.OwnerId == ownerId)
            .Include(g => g.GamePatterns)
            .ThenInclude(gp => gp.Pattern)
            .FirstOrDefaultAsync(ct);
}