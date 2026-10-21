using Bingora.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bingora.Infrastructure.Configurations;

public class PatternConfiguration : IEntityTypeConfiguration<Pattern>
{
    public void Configure(EntityTypeBuilder<Pattern> builder)
    {
        builder.ToTable("pattern", "bingo");

        builder.HasKey(p => p.Id);
        builder.Property(p => p.Name).HasMaxLength(100).IsRequired();
        builder.Property(p => p.GridRepresentation).HasMaxLength(25).IsRequired().IsUnicode(false);
        builder.Property(p => p.Bitmask).IsRequired();
        builder.Property(p => p.CreatedAt).IsRequired().HasDefaultValueSql("CURRENT_TIMESTAMP");
        builder.Property(p => p.IsActive);

        builder.HasIndex(p => p.Name).HasDatabaseName("IX_pattern_Name").IsUnique().HasDatabaseName("UQ_pattern_Name");
    }
}