namespace Bingora.Domain.Entities;

public class User
{
    public Guid Id { get; private set; }
    public string Username { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    public string? RefreshToken { get; private set; }
    public DateTime? RefreshTokenExpiration { get; private set; }

    public User(string username, string email, string passwordHash)
    {
        Id = Guid.NewGuid();
        Username = username;
        Email = email;
        PasswordHash = passwordHash;
        CreatedAt = DateTime.UtcNow;
    }

    public User()
    {
    }

    public void UpdateRefreshToken(string refreshToken, DateTime expiration)
    {
        RefreshToken = refreshToken;
        RefreshTokenExpiration = expiration;
    }

    public void RevokeRefreshToken()
    {
        RefreshToken = null;
        RefreshTokenExpiration = null;
    }
}