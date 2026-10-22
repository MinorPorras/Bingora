using Bingora.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bingora.Infrastructure.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> entity)
    {
        entity.ToTable("user", "bingo");

        entity.HasKey(x => x.Id);

        entity.Property(u => u.PasswordHash).HasColumnName("password_hash");
        entity.Property(u => u.CreatedAt).HasColumnName("created_at");
        entity.Property(u => u.IsActive).HasColumnName("is_active");

        entity.Property(u => u.Username).HasMaxLength(50).IsRequired();
        entity.Property(u => u.Email).HasMaxLength(255).IsRequired();

        entity.HasIndex(u => u.Username).IsUnique().HasDatabaseName("UQ_user_username");
        entity.HasIndex(u => u.Email).IsUnique().HasDatabaseName("UQ_user_email");

        entity.Property(u => u.PasswordHash).IsRequired();
        entity.Property(u => u.IsActive);
        entity.Property(u => u.Role).HasConversion<string>();

        entity.HasMany(u => u.CreatedBingos)
            .WithOne(b => b.Owner)
            .HasForeignKey(b => b.OwnerId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}