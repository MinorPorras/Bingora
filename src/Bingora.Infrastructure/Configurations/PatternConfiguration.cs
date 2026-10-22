using Bingora.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bingora.Infrastructure.Configurations;

public class PatternConfiguration : IEntityTypeConfiguration<Pattern>
{
    public void Configure(EntityTypeBuilder<Pattern> entity)
    {
        entity.ToTable("pattern", "bingo");

        entity.HasKey(p => p.Id);

        entity.Property(p => p.GridRepresentation).HasColumnName("grid_representation");
        entity.Property(p => p.CreatedAt).HasColumnName("created_at");
        entity.Property(p => p.IsActive).HasColumnName("is_active");

        entity.Property(p => p.Name).HasMaxLength(100).IsRequired();
        entity.HasIndex(p => p.Name).HasDatabaseName("IX_pattern_Name").IsUnique().HasDatabaseName("UQ_pattern_Name");

        entity.Property(p => p.GridRepresentation).HasMaxLength(25).IsRequired().IsUnicode(false);
        entity.Property(p => p.Bitmask).IsRequired();
        entity.Property(p => p.CreatedAt).IsRequired().HasDefaultValueSql("CURRENT_TIMESTAMP");
        entity.Property(p => p.IsActive);

        entity.HasMany(e => e.BingoPatterns)
            .WithOne(bp => bp.Pattern)
            .HasForeignKey(b => b.PatternId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}