using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bingora.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddWinnerToCorrectSquema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "winner",
                newName: "winner",
                newSchema: "bingo");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "winner",
                schema: "bingo",
                newName: "winner");
        }
    }
}
