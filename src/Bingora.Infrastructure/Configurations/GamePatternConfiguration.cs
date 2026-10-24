using Bingora.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bingora.Infrastructure.Configurations;

public class GamePatternConfiguration : IEntityTypeConfiguration<GamePattern>
{
    public void Configure(EntityTypeBuilder<GamePattern> entity)
    {
        entity.ToTable("game_pattern", "bingo");
        entity.HasKey(e => new { e.GameId, e.PatternId });

        entity.Property(e => e.GameId);
        entity.Property(e => e.PatternId);

        entity.Property(e => e.TotalPrizes).IsRequired();
        entity.Property(e => e.ClaimedPrizes).HasDefaultValue(0);

        entity.HasOne(e => e.Pattern)
            .WithMany()
            .HasForeignKey(e => e.PatternId)
            .OnDelete(DeleteBehavior.Restrict);

        entity.HasOne(e => e.Game)
            .WithMany(g => g.GamePatterns)
            .HasForeignKey(e => e.GameId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}