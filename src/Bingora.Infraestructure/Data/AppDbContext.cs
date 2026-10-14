using Bingora.Domain.Entities;
using Bingora.Infraestructure.Configurations;
using Microsoft.EntityFrameworkCore;

namespace Bingora.Infraestructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    // DbSets de las tablas
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Configuraciones personalizadas de cada entidad
        builder.ApplyConfiguration(new UserConfiguration());
    }
}