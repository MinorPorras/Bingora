using Bingora.Domain.Entities;
using Bingora.Infrastructure.Configurations;
using Microsoft.EntityFrameworkCore;

namespace Bingora.Infrastructure.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    // DbSets de las tablas
    public DbSet<Bingo> Bingos { get; set; }
    public DbSet<BingoPattern> BingoPatterns { get; set; }
    public DbSet<Card> Cards { get; set; }
    public DbSet<DrawnValue> DrawnValues { get; set; }
    public DbSet<Game> Games { get; set; }
    public DbSet<GamePattern> GamePatterns { get; set; }
    public DbSet<Pattern> Patterns { get; set; }
    public DbSet<Player> Players { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Winner> Winners { get; set; }


    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Configuraciones personalizadas de cada entidad
        builder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        //Global filter for softDelete
        builder.Entity<Bingo>().HasQueryFilter(b => b.IsActive);
        builder.Entity<Pattern>().HasQueryFilter(b => b.IsActive);
        builder.Entity<Card>().HasQueryFilter(b => b.IsActive);
        builder.Entity<User>().HasQueryFilter(b => b.IsActive);
        builder.Entity<Player>().HasQueryFilter(b => b.IsActive);
        builder.Entity<Winner>().HasQueryFilter(b => b.IsActive);
    }
}