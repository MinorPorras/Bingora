using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bingora.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangedIndexAndUniqueConstraintsInPatternName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameIndex(
                name: "IX_pattern_Name",
                schema: "bingo",
                table: "pattern",
                newName: "UQ_pattern_Name");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameIndex(
                name: "UQ_pattern_Name",
                schema: "bingo",
                table: "pattern",
                newName: "IX_pattern_Name");
        }
    }
}
