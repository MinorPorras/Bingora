using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bingora.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddBingoAndPatternRelationShip : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameIndex(
                name: "IX_user_Username",
                schema: "bingo",
                table: "user",
                newName: "UQ_user_username");

            migrationBuilder.RenameIndex(
                name: "IX_user_Email",
                schema: "bingo",
                table: "user",
                newName: "UQ_user_email");

            migrationBuilder.RenameIndex(
                name: "IX_refresh_token_Token",
                schema: "bingo",
                table: "refresh_token",
                newName: "UQ_refresh_token_token");

            migrationBuilder.AlterColumn<string>(
                name: "Role",
                schema: "bingo",
                table: "user",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                schema: "bingo",
                table: "user",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.CreateTable(
                name: "Bingo",
                schema: "bingo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    TotalCards = table.Column<int>(type: "integer", nullable: false),
                    OwnerId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    ColumnRanges = table.Column<string>(type: "jsonb", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bingo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Bingo_user_OwnerId",
                        column: x => x.OwnerId,
                        principalSchema: "bingo",
                        principalTable: "user",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "bingo_pattern",
                schema: "bingo",
                columns: table => new
                {
                    BingoId = table.Column<Guid>(type: "uuid", nullable: false),
                    PatternId = table.Column<Guid>(type: "uuid", nullable: false),
                    AvailablePrizes = table.Column<int>(type: "integer", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_bingo_pattern", x => new { x.BingoId, x.PatternId });
                    table.ForeignKey(
                        name: "FK_bingo_pattern_Bingo_BingoId",
                        column: x => x.BingoId,
                        principalSchema: "bingo",
                        principalTable: "Bingo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_bingo_pattern_pattern_PatternId",
                        column: x => x.PatternId,
                        principalSchema: "bingo",
                        principalTable: "pattern",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_bingo_name",
                schema: "bingo",
                table: "Bingo",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Bingo_OwnerId",
                schema: "bingo",
                table: "Bingo",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_bingo_pattern_PatternId",
                schema: "bingo",
                table: "bingo_pattern",
                column: "PatternId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "bingo_pattern",
                schema: "bingo");

            migrationBuilder.DropTable(
                name: "Bingo",
                schema: "bingo");

            migrationBuilder.RenameIndex(
                name: "UQ_user_username",
                schema: "bingo",
                table: "user",
                newName: "IX_user_Username");

            migrationBuilder.RenameIndex(
                name: "UQ_user_email",
                schema: "bingo",
                table: "user",
                newName: "IX_user_Email");

            migrationBuilder.RenameIndex(
                name: "UQ_refresh_token_token",
                schema: "bingo",
                table: "refresh_token",
                newName: "IX_refresh_token_Token");

            migrationBuilder.AlterColumn<int>(
                name: "Role",
                schema: "bingo",
                table: "user",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                schema: "bingo",
                table: "user",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(255)",
                oldMaxLength: 255);
        }
    }
}
