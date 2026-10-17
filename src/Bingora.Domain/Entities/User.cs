using Bingora.Domain.Enums;

namespace Bingora.Domain.Entities;

public class User
{
    public Guid Id { get; private set; }
    public string Username { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public bool IsActive { get; private set; }
    public Role Role { get; private set; } = Role.User;

    public User(string username, string email, string passwordHash, bool isActive, Role rol)
    {
        Id = Guid.NewGuid();
        Username = username;
        Email = email;
        PasswordHash = passwordHash;
        CreatedAt = DateTime.UtcNow;
        IsActive = isActive;
        Role = rol;
    }

    public User()
    {
    }

    public void Activate() => IsActive = true;
    public void Deactivate() => IsActive = false;
    public void UpdateUsername(string username) => Username = username;
    public void UpdateEmail(string email) => Email = email;
    public void UpdatePassword(string passwordHash) => PasswordHash = passwordHash;
    public void UpdateRole(Role role) => Role = role;
}