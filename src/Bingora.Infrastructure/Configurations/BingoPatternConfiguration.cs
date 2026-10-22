using Bingora.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bingora.Infrastructure.Configurations;

public class BingoPatternConfiguration : IEntityTypeConfiguration<BingoPattern>
{
    public void Configure(EntityTypeBuilder<BingoPattern> entity)
    {
        entity.ToTable("bingo_pattern",  "bingo");
        entity.HasKey(e => new  { e.BingoId, e.PatternId });
        
        entity.Property(e => e.AvailablePrizes).IsRequired().HasDefaultValue(0);
        
        entity.HasOne(e => e.Pattern)
            .WithMany(b => b.BingoPatterns)
            .HasForeignKey(e => e.PatternId)
            .OnDelete(DeleteBehavior.Restrict);
        
        entity.HasOne(e => e.Bingo)
            .WithMany(b => b.BingoPatterns)
            .HasForeignKey(e => e.BingoId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}