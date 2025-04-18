using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace WordSnapWeb.Migrations
{
    /// <inheritdoc />
    public partial class OldUserRemoved : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_cardsets_AspNetUsers_ApplicationUserId",
                table: "cardsets");

            migrationBuilder.DropForeignKey(
                name: "cardsets_user_ref_fkey",
                table: "cardsets");

            migrationBuilder.DropForeignKey(
                name: "FK_progress_AspNetUsers_ApplicationUserId",
                table: "progress");

            migrationBuilder.DropForeignKey(
                name: "progress_user_ref_fkey",
                table: "progress");

            migrationBuilder.DropForeignKey(
                name: "FK_userscardsets_AspNetUsers_ApplicationUserId",
                table: "userscardsets");

            migrationBuilder.DropForeignKey(
                name: "userscardsets_user_ref_fkey",
                table: "userscardsets");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropIndex(
                name: "IX_userscardsets_ApplicationUserId",
                table: "userscardsets");

            migrationBuilder.DropIndex(
                name: "IX_progress_ApplicationUserId",
                table: "progress");

            migrationBuilder.DropIndex(
                name: "IX_cardsets_ApplicationUserId",
                table: "cardsets");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "userscardsets");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "progress");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "cardsets");

            migrationBuilder.AlterColumn<string>(
                name: "user_ref",
                table: "userscardsets",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "user_ref",
                table: "progress",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "user_ref",
                table: "cardsets",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddForeignKey(
                name: "cardsets_user_ref_fkey",
                table: "cardsets",
                column: "user_ref",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "progress_user_ref_fkey",
                table: "progress",
                column: "user_ref",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "userscardsets_user_ref_fkey",
                table: "userscardsets",
                column: "user_ref",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "cardsets_user_ref_fkey",
                table: "cardsets");

            migrationBuilder.DropForeignKey(
                name: "progress_user_ref_fkey",
                table: "progress");

            migrationBuilder.DropForeignKey(
                name: "userscardsets_user_ref_fkey",
                table: "userscardsets");

            migrationBuilder.AlterColumn<int>(
                name: "user_ref",
                table: "userscardsets",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "userscardsets",
                type: "text",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "user_ref",
                table: "progress",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "progress",
                type: "text",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "user_ref",
                table: "cardsets",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "cardsets",
                type: "text",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "now()"),
                    email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    is_verified = table.Column<bool>(type: "boolean", nullable: true, defaultValue: false),
                    password_hash = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    password_salt = table.Column<string>(type: "character(24)", fixedLength: true, maxLength: 24, nullable: false),
                    username = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("users_pkey", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_userscardsets_ApplicationUserId",
                table: "userscardsets",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_progress_ApplicationUserId",
                table: "progress",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_cardsets_ApplicationUserId",
                table: "cardsets",
                column: "ApplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_cardsets_AspNetUsers_ApplicationUserId",
                table: "cardsets",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "cardsets_user_ref_fkey",
                table: "cardsets",
                column: "user_ref",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_progress_AspNetUsers_ApplicationUserId",
                table: "progress",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "progress_user_ref_fkey",
                table: "progress",
                column: "user_ref",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_userscardsets_AspNetUsers_ApplicationUserId",
                table: "userscardsets",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "userscardsets_user_ref_fkey",
                table: "userscardsets",
                column: "user_ref",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
