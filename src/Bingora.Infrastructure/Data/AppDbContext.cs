using Bingora.Domain.Entities;
using Bingora.Infrastructure.Configurations;
using Microsoft.EntityFrameworkCore;

namespace Bingora.Infrastructure.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    // DbSets de las tablas
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Configuraciones personalizadas de cada entidad
        builder.ApplyConfiguration(new UserConfiguration());
    }
}