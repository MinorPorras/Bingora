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

    //Navegación
    public IReadOnlyCollection<Bingo> CreatedBingos => _createdBingos.AsReadOnly();
    private List<Bingo> _createdBingos = [];

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

    public void SoftDelete()
    {
        if (!IsActive)
            throw new InvalidOperationException("El Bingo ya está eliminado.");
        IsActive = false;
    }

    public void Restore()
    {
        if (IsActive)
            throw new InvalidOperationException("El Bingo ya está activo.");
        IsActive = true;
    }

    public void UpdateUsername(string username) => Username = username;
    public void UpdateEmail(string email) => Email = email;
    public void UpdatePassword(string passwordHash) => PasswordHash = passwordHash;
    public void UpdateRole(Role role) => Role = role;
}