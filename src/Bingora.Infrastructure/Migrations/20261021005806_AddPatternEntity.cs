using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bingora.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPatternEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "pattern",
                schema: "bingo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    GridRepresentation = table.Column<string>(type: "character varying(25)", unicode: false, maxLength: 25, nullable: false),
                    Bitmask = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pattern", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_pattern_Name",
                schema: "bingo",
                table: "pattern",
                column: "Name");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "pattern",
                schema: "bingo");
        }
    }
}
