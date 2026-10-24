using Bingora.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bingora.Infrastructure.Configurations;

public class GameConfiguration : IEntityTypeConfiguration<Game>
{
    public void Configure(EntityTypeBuilder<Game> entity)
    {
        entity.ToTable("game", "bingo");
        entity.HasKey(e => e.Id);
        

        entity.HasIndex(g => g.BingoId).HasDatabaseName("IX_game_bingo_id");

        entity.Property(g => g.Title).HasMaxLength(150).IsRequired();
        entity.HasIndex(g => g.Title)
            .IsUnique()
            .HasDatabaseName("UQ_game_title");

        entity.Property(g => g.Status).HasConversion<string>().HasMaxLength(128).IsRequired();
        entity.Property(g => g.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
        entity.Property(g => g.StartedAt);
        entity.Property(g => g.FinishedAt);
        entity.Property(g => g.AnimationDuration).HasDefaultValue(3);
        entity.Property(g => g.WinnerSoundPath).HasMaxLength(500);

        entity.HasOne(g => g.Bingo)
            .WithMany(b => b.Games)
            .HasForeignKey(g => g.BingoId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}