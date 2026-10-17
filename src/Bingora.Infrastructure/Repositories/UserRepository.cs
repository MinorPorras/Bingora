using Bingora.Application.Interfaces;
using Bingora.Domain.Common;
using Bingora.Domain.Entities;
using Bingora.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Bingora.Infrastructure.Repositories;

public class UserRepository(AppDbContext context) : IUserRepository
{
    public async Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default(CancellationToken)) =>
        await context.Users.FirstOrDefaultAsync(u => u.Id == id, ct);

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

    public async Task<User?> GetByEmailAsync(string email, CancellationToken ct = default(CancellationToken)) =>
        await context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Email == email, ct);

    public async Task<User?> GetByUsernameAsync(string username, CancellationToken ct = default(CancellationToken)) =>
        await context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Username == username, ct);

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