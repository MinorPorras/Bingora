using Bingora.Application.Interfaces;
using Bingora.Domain.Entities;
using Bingora.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Bingora.Infrastructure.Repositories;

public class UserRepository(AppDbContext context) : IUserRepository
{
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