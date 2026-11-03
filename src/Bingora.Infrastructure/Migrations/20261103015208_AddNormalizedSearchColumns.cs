using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bingora.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddNormalizedSearchColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_bingo_name",
                schema: "bingo",
                table: "bingo");

            migrationBuilder.DropColumn(
                name: "player_id",
                schema: "bingo",
                table: "card");

            migrationBuilder.AddColumn<string>(
                name: "email_normalized",
                schema: "bingo",
                table: "user",
                type: "text",
                nullable: true,
                computedColumnSql: "LOWER(TRANSLATE(\"email\", 'áéíóúÁÉÍÓÚ', 'aeiouAEIOU'))",
                stored: true);

            migrationBuilder.AddColumn<string>(
                name: "username_normalized",
                schema: "bingo",
                table: "user",
                type: "text",
                nullable: true,
                computedColumnSql: "LOWER(TRANSLATE(\"username\", 'áéíóúÁÉÍÓÚ', 'aeiouAEIOU'))",
                stored: true);

            migrationBuilder.AddColumn<string>(
                name: "name_normalized",
                schema: "bingo",
                table: "pattern",
                type: "text",
                nullable: true,
                computedColumnSql: "LOWER(TRANSLATE(\"name\", 'áéíóúÁÉÍÓÚ', 'aeiouAEIOU'))",
                stored: true);

            migrationBuilder.AddColumn<string>(
                name: "title_normalized",
                schema: "bingo",
                table: "game",
                type: "text",
                nullable: true,
                computedColumnSql: "LOWER(TRANSLATE(\"title\", 'áéíóúÁÉÍÓÚ', 'aeiouAEIOU'))",
                stored: true);

            migrationBuilder.AddColumn<string>(
                name: "name_normalized",
                schema: "bingo",
                table: "bingo",
                type: "text",
                nullable: true,
                computedColumnSql: "LOWER(TRANSLATE(\"name\", 'áéíóúÁÉÍÓÚ', 'aeiouAEIOU'))",
                stored: true);

            migrationBuilder.CreateIndex(
                name: "IX_user_email_normalized",
                schema: "bingo",
                table: "user",
                column: "email_normalized");

            migrationBuilder.CreateIndex(
                name: "IX_user_username_normalized",
                schema: "bingo",
                table: "user",
                column: "username_normalized");

            migrationBuilder.CreateIndex(
                name: "IX_pattern_name_normalized",
                schema: "bingo",
                table: "pattern",
                column: "name_normalized");

            migrationBuilder.CreateIndex(
                name: "IX_game_title_normalized",
                schema: "bingo",
                table: "game",
                column: "title_normalized");

            migrationBuilder.CreateIndex(
                name: "IX_bingo_name_normalized",
                schema: "bingo",
                table: "bingo",
                column: "name_normalized");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_user_email_normalized",
                schema: "bingo",
                table: "user");

            migrationBuilder.DropIndex(
                name: "IX_user_username_normalized",
                schema: "bingo",
                table: "user");

            migrationBuilder.DropIndex(
                name: "IX_pattern_name_normalized",
                schema: "bingo",
                table: "pattern");

            migrationBuilder.DropIndex(
                name: "IX_game_title_normalized",
                schema: "bingo",
                table: "game");

            migrationBuilder.DropIndex(
                name: "IX_bingo_name_normalized",
                schema: "bingo",
                table: "bingo");

            migrationBuilder.DropColumn(
                name: "email_normalized",
                schema: "bingo",
                table: "user");

            migrationBuilder.DropColumn(
                name: "username_normalized",
                schema: "bingo",
                table: "user");

            migrationBuilder.DropColumn(
                name: "name_normalized",
                schema: "bingo",
                table: "pattern");

            migrationBuilder.DropColumn(
                name: "title_normalized",
                schema: "bingo",
                table: "game");

            migrationBuilder.DropColumn(
                name: "name_normalized",
                schema: "bingo",
                table: "bingo");

            migrationBuilder.AddColumn<Guid>(
                name: "player_id",
                schema: "bingo",
                table: "card",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_bingo_name",
                schema: "bingo",
                table: "bingo",
                column: "name");
        }
    }
}
