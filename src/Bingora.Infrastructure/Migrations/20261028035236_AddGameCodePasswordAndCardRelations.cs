using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bingora.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddGameCodePasswordAndCardRelations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_card_bingos_bingo_id",
                schema: "bingo",
                table: "card");

            migrationBuilder.DropForeignKey(
                name: "fk_drawn_value_game_game_id",
                schema: "bingo",
                table: "drawn_value");

            migrationBuilder.DropForeignKey(
                name: "fk_game_bingos_bingo_id",
                schema: "bingo",
                table: "game");

            migrationBuilder.DropForeignKey(
                name: "fk_winner_patterns_pattern_id",
                schema: "bingo",
                table: "winner");

            migrationBuilder.DropIndex(
                name: "UQ_game_title",
                schema: "bingo",
                table: "game");

            migrationBuilder.AlterColumn<string>(
                name: "status",
                schema: "bingo",
                table: "game",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(128)",
                oldMaxLength: 128);

            migrationBuilder.AddColumn<string>(
                name: "access_password",
                schema: "bingo",
                table: "game",
                type: "character varying(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "game_code",
                schema: "bingo",
                table: "game",
                type: "character varying(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "game_id",
                schema: "bingo",
                table: "card",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "player_id",
                schema: "bingo",
                table: "card",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "UQ_game_code",
                schema: "bingo",
                table: "game",
                column: "game_code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_card_game_id",
                schema: "bingo",
                table: "card",
                column: "game_id");

            migrationBuilder.AddForeignKey(
                name: "fk_card_bingo_bingo_id",
                schema: "bingo",
                table: "card",
                column: "bingo_id",
                principalSchema: "bingo",
                principalTable: "bingo",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_card_games_game_id",
                schema: "bingo",
                table: "card",
                column: "game_id",
                principalSchema: "bingo",
                principalTable: "game",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_drawn_value_games_game_id",
                schema: "bingo",
                table: "drawn_value",
                column: "game_id",
                principalSchema: "bingo",
                principalTable: "game",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_game_bingo_bingo_id",
                schema: "bingo",
                table: "game",
                column: "bingo_id",
                principalSchema: "bingo",
                principalTable: "bingo",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_winner_pattern_pattern_id",
                schema: "bingo",
                table: "winner",
                column: "pattern_id",
                principalSchema: "bingo",
                principalTable: "pattern",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_card_bingo_bingo_id",
                schema: "bingo",
                table: "card");

            migrationBuilder.DropForeignKey(
                name: "fk_card_games_game_id",
                schema: "bingo",
                table: "card");

            migrationBuilder.DropForeignKey(
                name: "fk_drawn_value_games_game_id",
                schema: "bingo",
                table: "drawn_value");

            migrationBuilder.DropForeignKey(
                name: "fk_game_bingo_bingo_id",
                schema: "bingo",
                table: "game");

            migrationBuilder.DropForeignKey(
                name: "fk_winner_pattern_pattern_id",
                schema: "bingo",
                table: "winner");

            migrationBuilder.DropIndex(
                name: "UQ_game_code",
                schema: "bingo",
                table: "game");

            migrationBuilder.DropIndex(
                name: "IX_card_game_id",
                schema: "bingo",
                table: "card");

            migrationBuilder.DropColumn(
                name: "access_password",
                schema: "bingo",
                table: "game");

            migrationBuilder.DropColumn(
                name: "game_code",
                schema: "bingo",
                table: "game");

            migrationBuilder.DropColumn(
                name: "game_id",
                schema: "bingo",
                table: "card");

            migrationBuilder.DropColumn(
                name: "player_id",
                schema: "bingo",
                table: "card");

            migrationBuilder.AlterColumn<string>(
                name: "status",
                schema: "bingo",
                table: "game",
                type: "character varying(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.CreateIndex(
                name: "UQ_game_title",
                schema: "bingo",
                table: "game",
                column: "title",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "fk_card_bingos_bingo_id",
                schema: "bingo",
                table: "card",
                column: "bingo_id",
                principalSchema: "bingo",
                principalTable: "bingo",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_drawn_value_game_game_id",
                schema: "bingo",
                table: "drawn_value",
                column: "game_id",
                principalSchema: "bingo",
                principalTable: "game",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_game_bingos_bingo_id",
                schema: "bingo",
                table: "game",
                column: "bingo_id",
                principalSchema: "bingo",
                principalTable: "bingo",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_winner_patterns_pattern_id",
                schema: "bingo",
                table: "winner",
                column: "pattern_id",
                principalSchema: "bingo",
                principalTable: "pattern",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
