using Bingora.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bingora.Infrastructure.Configurations;

public class PlayerConfiguration : IEntityTypeConfiguration<Player>
{
    public void Configure(EntityTypeBuilder<Player> entity)
    {
        entity.ToTable("player", "bingo");
        entity.HasKey(p => p.Id);

        entity.Property(p => p.Name).HasMaxLength(100).IsRequired();
        entity.Property(p => p.JoinedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
        entity.Property(p => p.IsActive).HasDefaultValue(true);

        entity.HasIndex(p => p.GameId).HasDatabaseName("IX_player_game_id");
        entity.HasIndex(p => p.CardId).HasDatabaseName("IX_player_card_id");

        entity.HasOne(p => p.Game)
            .WithMany(g => g.Players)
            .HasForeignKey(p => p.GameId)
            .OnDelete(DeleteBehavior.Cascade);

        entity.HasOne(p => p.Card)
            .WithOne(p => p.Player)
            .HasForeignKey<Player>(p => p.CardId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}