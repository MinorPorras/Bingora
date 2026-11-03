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

        entity.Property(p => p.Name).HasMaxLength(100).IsRequired();
        entity.HasIndex(p => p.Name).IsUnique().HasDatabaseName("UQ_pattern_Name");

        entity.Property<string>("name_normalized")
            .HasComputedColumnSql("LOWER(TRANSLATE(\"name\", 'áéíóúÁÉÍÓÚ', 'aeiouAEIOU'))", stored: true);
        entity.HasIndex("name_normalized").HasDatabaseName("IX_pattern_name_normalized");

        entity.Property(p => p.GridRepresentation).HasMaxLength(25).IsRequired().IsUnicode(false);
        entity.Property(p => p.Bitmask).IsRequired().HasColumnName("bitmask");
        entity.Property(p => p.CreatedAt).IsRequired().HasDefaultValueSql("CURRENT_TIMESTAMP");
        entity.Property(p => p.IsActive);

        entity.HasMany(e => e.BingoPatterns)
            .WithOne(bp => bp.Pattern)
            .HasForeignKey(b => b.PatternId)
            .OnDelete(DeleteBehavior.Cascade);

        entity.HasOne(e => e.Owner)
            .WithMany(u => u.CreatedPatterns)
            .HasForeignKey(b => b.OwnerId)
            .OnDelete(DeleteBehavior.Restrict);

        entity.HasIndex(p => p.OwnerId).HasDatabaseName("IX_pattern_owner_id");
    }
}