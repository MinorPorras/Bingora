using Bingora.Application.Interfaces;
using Bingora.Domain.Common;
using Bingora.Domain.Entities;
using Bingora.Infrastructure.Data;
using Bingora.Infrastructure.Helpers;
using Microsoft.EntityFrameworkCore;

namespace Bingora.Infrastructure.Repositories;

public class UserRepository(AppDbContext context) : IUserRepository
{
    public async Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default(CancellationToken)) =>
        await context.Users.FirstOrDefaultAsync(u => u.Id == id, ct);

    public async Task<User?> GetByIdAsyncIgnoringQueryFilters(Guid id, CancellationToken ct = default) =>
        await context.Users.IgnoreQueryFilters().FirstOrDefaultAsync(u => u.Id == id, ct);

    public async Task<PaginatedResult<User>> GetPagedAsync(int pageNumber, int pageSize,
        CancellationToken ct = default(CancellationToken))
    {
        var page = Math.Max(1, pageNumber);
        var size = Math.Clamp(pageSize, 1, 100);

        var query = context.Users.AsNoTracking().OrderBy(u => u.CreatedAt);

        var total = await query.CountAsync(ct);
        var items = await query.Skip((page - 1) * size).Take(size).ToListAsync(ct);

        return new PaginatedResult<User>(items, total, page, size);
    }

    public async Task<PaginatedResult<User>> GetPagedByUsernameOrEmailAsync(int pageNumber, int pageSize,
        string searchTerm,
        CancellationToken ct = default)
    {
        var page = Math.Max(1, pageNumber);
        var size = Math.Clamp(pageSize, 1, 100);
        var query = context.Users.AsNoTracking()
            .Where(u =>
                EF.Property<string>(u, "username_normalized").Contains(searchTerm.NormalizeSearchTerm()) ||
                EF.Property<string>(u, "email_normalized").Contains(searchTerm.NormalizeSearchTerm()))
            .OrderBy(u => u.CreatedAt);
        var total = await query.CountAsync(ct);
        var items = await query.Skip((page - 1) * size).Take(size).ToListAsync(ct);
        return new PaginatedResult<User>(items, page, total, size);
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken ct = default(CancellationToken)) =>
        await context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Email == email, ct);

    public async Task<User?> GetByUsernameAsync(string username, CancellationToken ct = default(CancellationToken)) =>
        await context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Username == username, ct);

    public async Task<User?> GetByEmailOrUsernameAsync(string input, CancellationToken ct = default)
    {
        var normalizedInput = input.ToLowerInvariant();
        return await context.Users.AsNoTracking()
            .FirstOrDefaultAsync(u =>
                    EF.Functions.ILike(u.Email, normalizedInput) ||
                    EF.Functions.ILike(u.Username, normalizedInput),
                ct);
    }


    public async Task AddAsync(User newUser, CancellationToken ct = default(CancellationToken))
    {
        await context.Users.AddAsync(newUser, ct);
        await context.SaveChangesAsync(ct);
    }

    public Task UpdateAsync(User updatedUser, CancellationToken ct = default(CancellationToken))
    {
        context.Users.Update(updatedUser);
        return context.SaveChangesAsync(ct);
    }
}