using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bingora.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCardGamePlayerWinnerBlock : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "card",
                schema: "bingo",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    bingo_id = table.Column<Guid>(type: "uuid", nullable: false),
                    code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    qr_code_path = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    values = table.Column<string>(type: "text", nullable: false),
                    bitmask = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_card", x => x.id);
                    table.ForeignKey(
                        name: "fk_card_bingos_bingo_id",
                        column: x => x.bingo_id,
                        principalSchema: "bingo",
                        principalTable: "bingo",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "player",
                schema: "bingo",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    game_id = table.Column<Guid>(type: "uuid", nullable: false),
                    card_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    joined_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_player", x => x.id);
                    table.ForeignKey(
                        name: "fk_player_card_card_id",
                        column: x => x.card_id,
                        principalSchema: "bingo",
                        principalTable: "card",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_player_game_game_id",
                        column: x => x.game_id,
                        principalSchema: "bingo",
                        principalTable: "game",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "winner",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    game_id = table.Column<Guid>(type: "uuid", nullable: false),
                    player_id = table.Column<Guid>(type: "uuid", nullable: false),
                    pattern_id = table.Column<Guid>(type: "uuid", nullable: false),
                    claim_status = table.Column<int>(type: "integer", nullable: false),
                    won_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    is_active = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_winner", x => x.id);
                    table.ForeignKey(
                        name: "fk_winner_game_game_id",
                        column: x => x.game_id,
                        principalSchema: "bingo",
                        principalTable: "game",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_winner_patterns_pattern_id",
                        column: x => x.pattern_id,
                        principalSchema: "bingo",
                        principalTable: "pattern",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_winner_player_player_id",
                        column: x => x.player_id,
                        principalSchema: "bingo",
                        principalTable: "player",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_card_bingo_id",
                schema: "bingo",
                table: "card",
                column: "bingo_id");

            migrationBuilder.CreateIndex(
                name: "UQ_card_code",
                schema: "bingo",
                table: "card",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_player_card_id",
                schema: "bingo",
                table: "player",
                column: "card_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_player_game_id",
                schema: "bingo",
                table: "player",
                column: "game_id");

            migrationBuilder.CreateIndex(
                name: "IX_winner_game_id",
                table: "winner",
                column: "game_id");

            migrationBuilder.CreateIndex(
                name: "ix_winner_pattern_id",
                table: "winner",
                column: "pattern_id");

            migrationBuilder.CreateIndex(
                name: "IX_winner_player_id",
                table: "winner",
                column: "player_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "winner");

            migrationBuilder.DropTable(
                name: "player",
                schema: "bingo");

            migrationBuilder.DropTable(
                name: "card",
                schema: "bingo");
        }
    }
}
