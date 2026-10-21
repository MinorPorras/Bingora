using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bingora.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueToPatternName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_pattern_Name",
                schema: "bingo",
                table: "pattern");

            migrationBuilder.CreateIndex(
                name: "IX_pattern_Name",
                schema: "bingo",
                table: "pattern",
                column: "Name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_pattern_Name",
                schema: "bingo",
                table: "pattern");

            migrationBuilder.CreateIndex(
                name: "IX_pattern_Name",
                schema: "bingo",
                table: "pattern",
                column: "Name");
        }
    }
}
