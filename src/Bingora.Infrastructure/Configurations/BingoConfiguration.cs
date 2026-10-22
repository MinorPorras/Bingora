using Bingora.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bingora.Infrastructure.Configurations;

public class BingoConfiguration : IEntityTypeConfiguration<Bingo>
{
    public void Configure(EntityTypeBuilder<Bingo> entity)
    {
        entity.ToTable("Bingo", "bingo");
        entity.HasKey(r => r.Id);
        
        entity.Property(e => e.Name).IsRequired().HasMaxLength(50);
        entity.HasIndex(e => e.Name).HasDatabaseName("IX_bingo_name");
        
        entity.Property(e => e.Description).HasMaxLength(500);
        entity.Property(e => e.TotalCards).IsRequired();
        entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

        entity.OwnsMany(b => b.ColumnRanges, cb =>
        {
            cb.ToJson();
        });
        
        entity.HasOne(r => r.Owner)
            .WithMany(u => u.CreatedBingos)
            .HasForeignKey(r => r.OwnerId)
            .OnDelete(DeleteBehavior.Restrict);
        
        entity.HasMany(b => b.BingoPatterns)
            .WithOne(bp => bp.Bingo)
            .HasForeignKey(b => b.BingoId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}