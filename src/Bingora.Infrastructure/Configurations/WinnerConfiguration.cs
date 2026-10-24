using Bingora.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bingora.Infrastructure.Configurations;

public class WinnerConfiguration : IEntityTypeConfiguration<Winner>
{
    public void Configure(EntityTypeBuilder<Winner> entity)
    {
        entity.ToTable("winner");
        entity.HasKey(w => w.Id);

        entity.Property(w => w.ClaimStatus).HasConversion<int>();
        entity.Property(w => w.WonAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

        entity.HasIndex(w => w.GameId).HasDatabaseName("IX_winner_game_id");
        entity.HasIndex(w => w.PlayerId).HasDatabaseName("IX_winner_player_id");

        entity.HasOne(w => w.Game)
            .WithMany(g => g.Winners)
            .HasForeignKey(w => w.GameId)
            .OnDelete(DeleteBehavior.Cascade);

        entity.HasOne(w => w.Player)
            .WithMany()
            .HasForeignKey(w => w.PlayerId)
            .OnDelete(DeleteBehavior.Restrict);

        entity.HasOne(w => w.Pattern)
            .WithMany()
            .HasForeignKey(w => w.PatternId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}