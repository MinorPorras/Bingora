using Bingora.Application.Interfaces;
using Bingora.Domain.Common;
using Bingora.Domain.Entities;
using Bingora.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Bingora.Infrastructure.Repositories;

public class PatternRepository(AppDbContext context) : IPatternRepository
{
    public async Task<Pattern?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        await context.Patterns.FirstOrDefaultAsync(p => p.Id == id, ct);

    public async Task<PaginatedResult<Pattern>> GetAllPagedAsync(int pageNumber, int pageSize,
        CancellationToken ct = default)
    {
        var page = Math.Max(1, pageNumber);
        var size = Math.Clamp(pageSize, 1, 100);

        var query = context.Patterns.AsNoTracking().OrderBy(p => p.Name);
        var total = await query.CountAsync(ct);

        var items = await query.Skip((page - 1) * size).Take(size).ToListAsync(ct);

        return new PaginatedResult<Pattern>(items, page, total, size);
    }

    public async Task<PaginatedResult<Pattern>> GetByNamePagedAsync(int pageNumber, int pageSize, string patternName,
        CancellationToken ct = default)
    {
        var page = Math.Max(1, pageNumber);
        var size = Math.Clamp(pageSize, 1, 100);

        var query = context.Patterns
            .AsNoTracking()
            .Where(p => p.Name.Contains(patternName))
            .OrderBy(p => p.Name);

        var total = await query.CountAsync(ct);

        var items = await query.Skip((page - 1) * size).Take(size).ToListAsync(ct);

        return new PaginatedResult<Pattern>(items, page, total, size);
    }

    public async Task AddAsync(Pattern pattern, CancellationToken ct = default)
    {
        context.Patterns.Add(pattern);
        await context.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(Pattern pattern, CancellationToken ct = default)
    {
        context.Patterns.Update(pattern);
        await context.SaveChangesAsync(ct);
    }
}