using Bingora.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bingora.Infrastructure.Configurations;

public class CardConfiguration : IEntityTypeConfiguration<Card>
{
    public void Configure(EntityTypeBuilder<Card> entity)
    {
        entity.ToTable("card", "bingo");
        entity.HasKey(x => x.Id);

        entity.HasIndex(c => c.BingoId).HasDatabaseName("IX_card_bingo_id");
        entity.HasIndex(c => c.GameId).HasDatabaseName("IX_card_game_id");
        entity.HasIndex(x => x.Code).IsUnique().HasDatabaseName("UQ_card_code");
        
        entity.Property(c => c.Code).HasMaxLength(50).IsRequired();
        entity.Property(c => c.QrCodePath).HasMaxLength(500);
        entity.Property(c => c.Values).IsRequired();
        entity.Property(c => c.Bitmask).IsRequired();
        entity.Property(c => c.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
        entity.Property(c => c.IsActive).HasDefaultValue(true);

        entity.HasOne(c => c.Bingo)
            .WithMany(b => b.Cards)
            .HasForeignKey(c => c.BingoId)
            .OnDelete(DeleteBehavior.Restrict);

        entity.HasOne(c => c.Game)
            .WithMany(g => g.Cards)
            .HasForeignKey(c => c.GameId)
            .OnDelete(DeleteBehavior.Cascade);

        entity.HasOne(c => c.Player)
            .WithOne(p => p.Card)
            .HasForeignKey<Player>(p => p.CardId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}