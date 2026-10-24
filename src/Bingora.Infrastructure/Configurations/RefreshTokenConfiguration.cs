using Bingora.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bingora.Infrastructure.Configurations;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> entity)
    {
        entity.ToTable("refresh_token", "bingo");

        entity.HasKey(r => r.Id);

        entity.Property(r => r.Token).IsRequired().HasMaxLength(500);
        entity.HasIndex(r => r.Token).IsUnique().HasDatabaseName("UQ_refresh_token_token");

        entity.HasOne(r => r.User)
            .WithMany().HasForeignKey(r => r.UserId).OnDelete(DeleteBehavior.Cascade);

        entity.Property(r => r.CreatedAt).IsRequired();
        entity.Property(r => r.ExpiresAt).IsRequired();
        entity.Property(r => r.RevokedAt);
        entity.Property(r => r.ReplacedByTokenId);
    }
}