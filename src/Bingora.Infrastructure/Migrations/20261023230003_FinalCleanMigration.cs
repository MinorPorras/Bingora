using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bingora.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FinalCleanMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "bingo");

            migrationBuilder.CreateTable(
                name: "pattern",
                schema: "bingo",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    grid_representation = table.Column<string>(type: "character varying(25)", unicode: false, maxLength: 25, nullable: false),
                    bitmask = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    is_active = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_pattern", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "user",
                schema: "bingo",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    username = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    password_hash = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    role = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "bingo",
                schema: "bingo",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    total_cards = table.Column<int>(type: "integer", nullable: false),
                    owner_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    column_ranges = table.Column<string>(type: "jsonb", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_bingo", x => x.id);
                    table.ForeignKey(
                        name: "fk_bingo_users_owner_id",
                        column: x => x.owner_id,
                        principalSchema: "bingo",
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "refresh_token",
                schema: "bingo",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    token = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    expires_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    revoked_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    replaced_by_token_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_refresh_token", x => x.id);
                    table.ForeignKey(
                        name: "fk_refresh_token_users_user_id",
                        column: x => x.user_id,
                        principalSchema: "bingo",
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "bingo_pattern",
                schema: "bingo",
                columns: table => new
                {
                    bingo_id = table.Column<Guid>(type: "uuid", nullable: false),
                    pattern_id = table.Column<Guid>(type: "uuid", nullable: false),
                    available_prizes = table.Column<int>(type: "integer", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_bingo_pattern", x => new { x.bingo_id, x.pattern_id });
                    table.ForeignKey(
                        name: "fk_bingo_pattern_bingo_bingo_id",
                        column: x => x.bingo_id,
                        principalSchema: "bingo",
                        principalTable: "bingo",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_bingo_pattern_patterns_pattern_id",
                        column: x => x.pattern_id,
                        principalSchema: "bingo",
                        principalTable: "pattern",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "game",
                schema: "bingo",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    bingo_id = table.Column<Guid>(type: "uuid", nullable: false),
                    title = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    status = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    started_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    finished_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    animation_duration = table.Column<int>(type: "integer", nullable: false, defaultValue: 3),
                    winner_sound_path = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_game", x => x.id);
                    table.ForeignKey(
                        name: "fk_game_bingos_bingo_id",
                        column: x => x.bingo_id,
                        principalSchema: "bingo",
                        principalTable: "bingo",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "drawn_value",
                schema: "bingo",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    game_id = table.Column<Guid>(type: "uuid", nullable: false),
                    value = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    draw_order = table.Column<int>(type: "integer", nullable: false),
                    drawn_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_drawn_value", x => x.id);
                    table.ForeignKey(
                        name: "fk_drawn_value_game_game_id",
                        column: x => x.game_id,
                        principalSchema: "bingo",
                        principalTable: "game",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "game_pattern",
                schema: "bingo",
                columns: table => new
                {
                    game_id = table.Column<Guid>(type: "uuid", nullable: false),
                    pattern_id = table.Column<Guid>(type: "uuid", nullable: false),
                    total_prizes = table.Column<int>(type: "integer", nullable: false),
                    claimed_prizes = table.Column<int>(type: "integer", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_game_pattern", x => new { x.game_id, x.pattern_id });
                    table.ForeignKey(
                        name: "fk_game_pattern_game_game_id",
                        column: x => x.game_id,
                        principalSchema: "bingo",
                        principalTable: "game",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_game_pattern_patterns_pattern_id",
                        column: x => x.pattern_id,
                        principalSchema: "bingo",
                        principalTable: "pattern",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_bingo_name",
                schema: "bingo",
                table: "bingo",
                column: "name");

            migrationBuilder.CreateIndex(
                name: "ix_bingo_owner_id",
                schema: "bingo",
                table: "bingo",
                column: "owner_id");

            migrationBuilder.CreateIndex(
                name: "ix_bingo_pattern_pattern_id",
                schema: "bingo",
                table: "bingo_pattern",
                column: "pattern_id");

            migrationBuilder.CreateIndex(
                name: "IX_drawn_value_game_order",
                schema: "bingo",
                table: "drawn_value",
                columns: new[] { "game_id", "draw_order" });

            migrationBuilder.CreateIndex(
                name: "IX_game_bingo_id",
                schema: "bingo",
                table: "game",
                column: "bingo_id");

            migrationBuilder.CreateIndex(
                name: "UQ_game_title",
                schema: "bingo",
                table: "game",
                column: "title",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_game_pattern_pattern_id",
                schema: "bingo",
                table: "game_pattern",
                column: "pattern_id");

            migrationBuilder.CreateIndex(
                name: "UQ_pattern_Name",
                schema: "bingo",
                table: "pattern",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_refresh_token_user_id",
                schema: "bingo",
                table: "refresh_token",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "UQ_refresh_token_token",
                schema: "bingo",
                table: "refresh_token",
                column: "token",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ_user_email",
                schema: "bingo",
                table: "user",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ_user_username",
                schema: "bingo",
                table: "user",
                column: "username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "bingo_pattern",
                schema: "bingo");

            migrationBuilder.DropTable(
                name: "drawn_value",
                schema: "bingo");

            migrationBuilder.DropTable(
                name: "game_pattern",
                schema: "bingo");

            migrationBuilder.DropTable(
                name: "refresh_token",
                schema: "bingo");

            migrationBuilder.DropTable(
                name: "game",
                schema: "bingo");

            migrationBuilder.DropTable(
                name: "pattern",
                schema: "bingo");

            migrationBuilder.DropTable(
                name: "bingo",
                schema: "bingo");

            migrationBuilder.DropTable(
                name: "user",
                schema: "bingo");
        }
    }
}
