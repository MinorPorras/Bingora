using Bingora.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bingora.Infrastructure.Configurations;

public class DrawnValueConfiguration : IEntityTypeConfiguration<DrawnValue>
{
    public void Configure(EntityTypeBuilder<DrawnValue> entity)
    {
        entity.ToTable("drawn_value", "bingo");
        entity.HasKey(d => d.Id);

        entity.Property(d => d.GameId).HasColumnName("game_id");
        entity.Property(d => d.DrawOrder).HasColumnName("draw_order");
        entity.Property(d => d.DrawnAt).HasColumnName("drawn_at");

        entity.Property(d => d.Value).HasMaxLength(50).IsRequired();
        entity.Property(e => e.DrawOrder).IsRequired();
        entity.Property(e => e.DrawnAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

        entity.HasIndex(e => new { e.GameId, e.DrawOrder }).HasDatabaseName("IX_drawn_value_game_order");

        entity.HasOne(e => e.Game)
            .WithMany(g => g.DrawnValues)
            .HasForeignKey(e => e.GameId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}