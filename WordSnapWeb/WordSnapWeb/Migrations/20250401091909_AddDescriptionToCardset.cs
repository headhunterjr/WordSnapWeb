using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WordSnapWeb.Migrations
{
    /// <inheritdoc />
    public partial class AddDescriptionToCardset : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "cardsets",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "cardsets");
        }
    }
}
