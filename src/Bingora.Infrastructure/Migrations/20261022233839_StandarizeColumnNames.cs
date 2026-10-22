using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bingora.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class StandarizeColumnNames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bingo_user_OwnerId",
                schema: "bingo",
                table: "Bingo");

            migrationBuilder.DropForeignKey(
                name: "FK_bingo_pattern_Bingo_BingoId",
                schema: "bingo",
                table: "bingo_pattern");

            migrationBuilder.DropForeignKey(
                name: "FK_bingo_pattern_pattern_PatternId",
                schema: "bingo",
                table: "bingo_pattern");

            migrationBuilder.DropForeignKey(
                name: "FK_drawn_value_game_GameId",
                schema: "bingo",
                table: "drawn_value");

            migrationBuilder.DropForeignKey(
                name: "FK_refresh_token_user_UserId",
                schema: "bingo",
                table: "refresh_token");

            migrationBuilder.RenameColumn(
                name: "PasswordHash",
                schema: "bingo",
                table: "user",
                newName: "password_hash");

            migrationBuilder.RenameColumn(
                name: "IsActive",
                schema: "bingo",
                table: "user",
                newName: "is_active");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                schema: "bingo",
                table: "user",
                newName: "created_at");

            migrationBuilder.RenameColumn(
                name: "UserId",
                schema: "bingo",
                table: "refresh_token",
                newName: "user_id");

            migrationBuilder.RenameColumn(
                name: "RevokedAt",
                schema: "bingo",
                table: "refresh_token",
                newName: "revoked_at");

            migrationBuilder.RenameColumn(
                name: "ReplacedByTokenId",
                schema: "bingo",
                table: "refresh_token",
                newName: "replaced_token_id");

            migrationBuilder.RenameColumn(
                name: "ExpiresAt",
                schema: "bingo",
                table: "refresh_token",
                newName: "expires_at");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                schema: "bingo",
                table: "refresh_token",
                newName: "created_at");

            migrationBuilder.RenameIndex(
                name: "IX_refresh_token_UserId",
                schema: "bingo",
                table: "refresh_token",
                newName: "IX_refresh_token_user_id");

            migrationBuilder.RenameColumn(
                name: "IsActive",
                schema: "bingo",
                table: "pattern",
                newName: "is_active");

            migrationBuilder.RenameColumn(
                name: "GridRepresentation",
                schema: "bingo",
                table: "pattern",
                newName: "grid_representation");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                schema: "bingo",
                table: "pattern",
                newName: "created_at");

            migrationBuilder.RenameColumn(
                name: "TotalPrizes",
                schema: "bingo",
                table: "game_pattern",
                newName: "total_prizes");

            migrationBuilder.RenameColumn(
                name: "ClaimedPrizes",
                schema: "bingo",
                table: "game_pattern",
                newName: "claimed_prizes");

            migrationBuilder.RenameColumn(
                name: "WinnerSoundPath",
                schema: "bingo",
                table: "game",
                newName: "winner_sound_path");

            migrationBuilder.RenameColumn(
                name: "StartedAt",
                schema: "bingo",
                table: "game",
                newName: "started_at");

            migrationBuilder.RenameColumn(
                name: "FinishedAt",
                schema: "bingo",
                table: "game",
                newName: "finished_at");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                schema: "bingo",
                table: "game",
                newName: "created_at");

            migrationBuilder.RenameColumn(
                name: "AnimationDuration",
                schema: "bingo",
                table: "game",
                newName: "animation_duration");

            migrationBuilder.RenameColumn(
                name: "GameId",
                schema: "bingo",
                table: "drawn_value",
                newName: "game_id");

            migrationBuilder.RenameColumn(
                name: "DrawnAt",
                schema: "bingo",
                table: "drawn_value",
                newName: "drawn_at");

            migrationBuilder.RenameColumn(
                name: "DrawOrder",
                schema: "bingo",
                table: "drawn_value",
                newName: "draw_order");

            migrationBuilder.RenameColumn(
                name: "AvailablePrizes",
                schema: "bingo",
                table: "bingo_pattern",
                newName: "available_prizes");

            migrationBuilder.RenameColumn(
                name: "PatternId",
                schema: "bingo",
                table: "bingo_pattern",
                newName: "pattern_id");

            migrationBuilder.RenameColumn(
                name: "BingoId",
                schema: "bingo",
                table: "bingo_pattern",
                newName: "bingo_id");

            migrationBuilder.RenameIndex(
                name: "IX_bingo_pattern_PatternId",
                schema: "bingo",
                table: "bingo_pattern",
                newName: "IX_bingo_pattern_pattern_id");

            migrationBuilder.RenameColumn(
                name: "TotalCards",
                schema: "bingo",
                table: "Bingo",
                newName: "total_cards");

            migrationBuilder.RenameColumn(
                name: "OwnerId",
                schema: "bingo",
                table: "Bingo",
                newName: "owner_id");

            migrationBuilder.RenameColumn(
                name: "IsActive",
                schema: "bingo",
                table: "Bingo",
                newName: "is_active");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                schema: "bingo",
                table: "Bingo",
                newName: "created_at");

            migrationBuilder.RenameIndex(
                name: "IX_Bingo_OwnerId",
                schema: "bingo",
                table: "Bingo",
                newName: "IX_Bingo_owner_id");

            migrationBuilder.AddForeignKey(
                name: "FK_Bingo_user_owner_id",
                schema: "bingo",
                table: "Bingo",
                column: "owner_id",
                principalSchema: "bingo",
                principalTable: "user",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_bingo_pattern_Bingo_bingo_id",
                schema: "bingo",
                table: "bingo_pattern",
                column: "bingo_id",
                principalSchema: "bingo",
                principalTable: "Bingo",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_bingo_pattern_pattern_pattern_id",
                schema: "bingo",
                table: "bingo_pattern",
                column: "pattern_id",
                principalSchema: "bingo",
                principalTable: "pattern",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_drawn_value_game_game_id",
                schema: "bingo",
                table: "drawn_value",
                column: "game_id",
                principalSchema: "bingo",
                principalTable: "game",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_refresh_token_user_user_id",
                schema: "bingo",
                table: "refresh_token",
                column: "user_id",
                principalSchema: "bingo",
                principalTable: "user",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bingo_user_owner_id",
                schema: "bingo",
                table: "Bingo");

            migrationBuilder.DropForeignKey(
                name: "FK_bingo_pattern_Bingo_bingo_id",
                schema: "bingo",
                table: "bingo_pattern");

            migrationBuilder.DropForeignKey(
                name: "FK_bingo_pattern_pattern_pattern_id",
                schema: "bingo",
                table: "bingo_pattern");

            migrationBuilder.DropForeignKey(
                name: "FK_drawn_value_game_game_id",
                schema: "bingo",
                table: "drawn_value");

            migrationBuilder.DropForeignKey(
                name: "FK_refresh_token_user_user_id",
                schema: "bingo",
                table: "refresh_token");

            migrationBuilder.RenameColumn(
                name: "password_hash",
                schema: "bingo",
                table: "user",
                newName: "PasswordHash");

            migrationBuilder.RenameColumn(
                name: "is_active",
                schema: "bingo",
                table: "user",
                newName: "IsActive");

            migrationBuilder.RenameColumn(
                name: "created_at",
                schema: "bingo",
                table: "user",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "user_id",
                schema: "bingo",
                table: "refresh_token",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "revoked_at",
                schema: "bingo",
                table: "refresh_token",
                newName: "RevokedAt");

            migrationBuilder.RenameColumn(
                name: "replaced_token_id",
                schema: "bingo",
                table: "refresh_token",
                newName: "ReplacedByTokenId");

            migrationBuilder.RenameColumn(
                name: "expires_at",
                schema: "bingo",
                table: "refresh_token",
                newName: "ExpiresAt");

            migrationBuilder.RenameColumn(
                name: "created_at",
                schema: "bingo",
                table: "refresh_token",
                newName: "CreatedAt");

            migrationBuilder.RenameIndex(
                name: "IX_refresh_token_user_id",
                schema: "bingo",
                table: "refresh_token",
                newName: "IX_refresh_token_UserId");

            migrationBuilder.RenameColumn(
                name: "is_active",
                schema: "bingo",
                table: "pattern",
                newName: "IsActive");

            migrationBuilder.RenameColumn(
                name: "grid_representation",
                schema: "bingo",
                table: "pattern",
                newName: "GridRepresentation");

            migrationBuilder.RenameColumn(
                name: "created_at",
                schema: "bingo",
                table: "pattern",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "total_prizes",
                schema: "bingo",
                table: "game_pattern",
                newName: "TotalPrizes");

            migrationBuilder.RenameColumn(
                name: "claimed_prizes",
                schema: "bingo",
                table: "game_pattern",
                newName: "ClaimedPrizes");

            migrationBuilder.RenameColumn(
                name: "winner_sound_path",
                schema: "bingo",
                table: "game",
                newName: "WinnerSoundPath");

            migrationBuilder.RenameColumn(
                name: "started_at",
                schema: "bingo",
                table: "game",
                newName: "StartedAt");

            migrationBuilder.RenameColumn(
                name: "finished_at",
                schema: "bingo",
                table: "game",
                newName: "FinishedAt");

            migrationBuilder.RenameColumn(
                name: "created_at",
                schema: "bingo",
                table: "game",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "animation_duration",
                schema: "bingo",
                table: "game",
                newName: "AnimationDuration");

            migrationBuilder.RenameColumn(
                name: "game_id",
                schema: "bingo",
                table: "drawn_value",
                newName: "GameId");

            migrationBuilder.RenameColumn(
                name: "drawn_at",
                schema: "bingo",
                table: "drawn_value",
                newName: "DrawnAt");

            migrationBuilder.RenameColumn(
                name: "draw_order",
                schema: "bingo",
                table: "drawn_value",
                newName: "DrawOrder");

            migrationBuilder.RenameColumn(
                name: "available_prizes",
                schema: "bingo",
                table: "bingo_pattern",
                newName: "AvailablePrizes");

            migrationBuilder.RenameColumn(
                name: "pattern_id",
                schema: "bingo",
                table: "bingo_pattern",
                newName: "PatternId");

            migrationBuilder.RenameColumn(
                name: "bingo_id",
                schema: "bingo",
                table: "bingo_pattern",
                newName: "BingoId");

            migrationBuilder.RenameIndex(
                name: "IX_bingo_pattern_pattern_id",
                schema: "bingo",
                table: "bingo_pattern",
                newName: "IX_bingo_pattern_PatternId");

            migrationBuilder.RenameColumn(
                name: "total_cards",
                schema: "bingo",
                table: "Bingo",
                newName: "TotalCards");

            migrationBuilder.RenameColumn(
                name: "owner_id",
                schema: "bingo",
                table: "Bingo",
                newName: "OwnerId");

            migrationBuilder.RenameColumn(
                name: "is_active",
                schema: "bingo",
                table: "Bingo",
                newName: "IsActive");

            migrationBuilder.RenameColumn(
                name: "created_at",
                schema: "bingo",
                table: "Bingo",
                newName: "CreatedAt");

            migrationBuilder.RenameIndex(
                name: "IX_Bingo_owner_id",
                schema: "bingo",
                table: "Bingo",
                newName: "IX_Bingo_OwnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Bingo_user_OwnerId",
                schema: "bingo",
                table: "Bingo",
                column: "OwnerId",
                principalSchema: "bingo",
                principalTable: "user",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_bingo_pattern_Bingo_BingoId",
                schema: "bingo",
                table: "bingo_pattern",
                column: "BingoId",
                principalSchema: "bingo",
                principalTable: "Bingo",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_bingo_pattern_pattern_PatternId",
                schema: "bingo",
                table: "bingo_pattern",
                column: "PatternId",
                principalSchema: "bingo",
                principalTable: "pattern",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_drawn_value_game_GameId",
                schema: "bingo",
                table: "drawn_value",
                column: "GameId",
                principalSchema: "bingo",
                principalTable: "game",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_refresh_token_user_UserId",
                schema: "bingo",
                table: "refresh_token",
                column: "UserId",
                principalSchema: "bingo",
                principalTable: "user",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
