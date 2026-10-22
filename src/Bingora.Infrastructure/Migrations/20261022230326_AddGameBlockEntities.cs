using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bingora.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddGameBlockEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "game",
                schema: "bingo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    bingo_id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Status = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    StartedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FinishedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    AnimationDuration = table.Column<int>(type: "integer", nullable: false, defaultValue: 3),
                    WinnerSoundPath = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_game", x => x.Id);
                    table.ForeignKey(
                        name: "FK_game_Bingo_bingo_id",
                        column: x => x.bingo_id,
                        principalSchema: "bingo",
                        principalTable: "Bingo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "drawn_value",
                schema: "bingo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    GameId = table.Column<Guid>(type: "uuid", nullable: false),
                    Value = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    DrawOrder = table.Column<int>(type: "integer", nullable: false),
                    DrawnAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_drawn_value", x => x.Id);
                    table.ForeignKey(
                        name: "FK_drawn_value_game_GameId",
                        column: x => x.GameId,
                        principalSchema: "bingo",
                        principalTable: "game",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "game_pattern",
                schema: "bingo",
                columns: table => new
                {
                    game_id = table.Column<Guid>(type: "uuid", nullable: false),
                    pattern_id = table.Column<Guid>(type: "uuid", nullable: false),
                    TotalPrizes = table.Column<int>(type: "integer", nullable: false),
                    ClaimedPrizes = table.Column<int>(type: "integer", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_game_pattern", x => new { x.game_id, x.pattern_id });
                    table.ForeignKey(
                        name: "FK_game_pattern_game_game_id",
                        column: x => x.game_id,
                        principalSchema: "bingo",
                        principalTable: "game",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_game_pattern_pattern_pattern_id",
                        column: x => x.pattern_id,
                        principalSchema: "bingo",
                        principalTable: "pattern",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_drawn_value_game_order",
                schema: "bingo",
                table: "drawn_value",
                columns: new[] { "GameId", "DrawOrder" });

            migrationBuilder.CreateIndex(
                name: "IX_game_bingo_id",
                schema: "bingo",
                table: "game",
                column: "bingo_id");

            migrationBuilder.CreateIndex(
                name: "UQ_game_title",
                schema: "bingo",
                table: "game",
                column: "Title",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_game_pattern_pattern_id",
                schema: "bingo",
                table: "game_pattern",
                column: "pattern_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "drawn_value",
                schema: "bingo");

            migrationBuilder.DropTable(
                name: "game_pattern",
                schema: "bingo");

            migrationBuilder.DropTable(
                name: "game",
                schema: "bingo");
        }
    }
}
