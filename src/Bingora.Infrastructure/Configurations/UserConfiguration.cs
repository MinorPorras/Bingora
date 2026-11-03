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

        entity.Property(u => u.Username).HasMaxLength(50).IsRequired();
        entity.Property(u => u.Email).HasMaxLength(255).IsRequired();

        entity.HasIndex(u => u.Username).IsUnique().HasDatabaseName("UQ_user_username");
        entity.HasIndex(u => u.Email).IsUnique().HasDatabaseName("UQ_user_email");

        entity.Property<string>("username_normalized")
            .HasComputedColumnSql("LOWER(TRANSLATE(\"username\", 'áéíóúÁÉÍÓÚ', 'aeiouAEIOU'))", stored: true);
        entity.HasIndex("username_normalized").HasDatabaseName("IX_user_username_normalized");

        entity.Property<string>("email_normalized")
            .HasComputedColumnSql("LOWER(TRANSLATE(\"email\", 'áéíóúÁÉÍÓÚ', 'aeiouAEIOU'))", stored: true);
        entity.HasIndex("email_normalized").HasDatabaseName("IX_user_email_normalized");

        entity.Property(u => u.Role).HasConversion<string>();

        entity.HasMany(u => u.CreatedBingos)
            .WithOne(b => b.Owner)
            .HasForeignKey(b => b.OwnerId)
            .OnDelete(DeleteBehavior.Restrict);

        entity.HasMany(u => u.CreatedPatterns)
            .WithOne(b => b.Owner)
            .HasForeignKey(b => b.OwnerId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}