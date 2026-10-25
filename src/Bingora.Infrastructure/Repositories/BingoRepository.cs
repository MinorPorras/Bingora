using Bingora.Application.DTOs.Bingo;
using Bingora.Application.Interfaces;
using Bingora.Domain.Common;
using Bingora.Domain.Entities;
using Bingora.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Bingora.Infrastructure.Repositories;

public class BingoRepository(AppDbContext context) : IBingoRepository
{
    public async Task<PaginatedResult<Bingo>> GetPagedAsync(Guid ownerId, int pageNumber, int pageSize,
        CancellationToken ct = default)
    {
        var page = Math.Max(1, pageNumber);
        var size = Math.Clamp(pageSize, 1, 100);

        var query = context.Bingos.AsNoTracking()
            .Where(b => b.OwnerId == ownerId)
            .OrderBy(x => x.CreatedAt);

        var total = await query.CountAsync(ct);
        var items = await query.Skip((page - 1) * size).Take(size).ToListAsync(ct);
        return new PaginatedResult<Bingo>(items, page, total, size);
    }

    public async Task<Bingo?> GetByIdAsync(Guid id, Guid ownerId, CancellationToken ct = default) =>
        await context.Bingos.Include(b => b.BingoPatterns).ThenInclude(bp => bp.Pattern)
            .FirstOrDefaultAsync(x => x.Id == id && x.OwnerId == ownerId, ct);

    public async Task<PaginatedResult<Bingo>> GetPagedByNameAsync(Guid ownerId, string name, int pageNumber,
        int pageSize,
        CancellationToken ct = default)
    {
        var page = Math.Max(1, pageNumber);
        var size = Math.Clamp(pageSize, 1, 100);

        var query = context.Bingos.AsNoTracking()
            .Where(b => b.Name.Contains(name) && b.OwnerId == ownerId)
            .OrderBy(x => x.CreatedAt);

        var total = await query.CountAsync(ct);
        var items = await query.Skip((page - 1) * size).Take(size).ToListAsync(ct);
        return new PaginatedResult<Bingo>(items, page, total, size);
    }

    public async Task AddAsync(Bingo bingo, CancellationToken ct = default)
    {
        context.Bingos.Add(bingo);
        await context.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(Bingo bingo, CancellationToken ct = default)
    {
        context.Bingos.Update(bingo);
        await context.SaveChangesAsync(ct);
    }
}