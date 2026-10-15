using Bingora.Domain.Entities;

namespace Bingora.Application.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email, CancellationToken ct = default(CancellationToken));
    Task<User?> GetByUsernameAsync(string username, CancellationToken ct = default(CancellationToken));
    Task<User?> GetByRefreshTokenAsync(string refreshToken, CancellationToken ct = default(CancellationToken));
    Task AddAsync(User newUser, CancellationToken ct = default(CancellationToken));
    Task UpdateAsync(User updatedUser, CancellationToken ct = default(CancellationToken));
}