using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bingora.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddOwnerIdToPatterns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "owner_id",
                schema: "bingo",
                table: "pattern",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_pattern_owner_id",
                schema: "bingo",
                table: "pattern",
                column: "owner_id");

            migrationBuilder.AddForeignKey(
                name: "fk_pattern_users_owner_id",
                schema: "bingo",
                table: "pattern",
                column: "owner_id",
                principalSchema: "bingo",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_pattern_users_owner_id",
                schema: "bingo",
                table: "pattern");

            migrationBuilder.DropIndex(
                name: "IX_pattern_owner_id",
                schema: "bingo",
                table: "pattern");

            migrationBuilder.DropColumn(
                name: "owner_id",
                schema: "bingo",
                table: "pattern");
        }
    }
}
