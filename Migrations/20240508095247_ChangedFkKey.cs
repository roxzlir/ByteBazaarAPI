using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ByteBazaarAPI.Migrations
{
    /// <inheritdoc />
    public partial class ChangedFkKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FkProductImage",
                table: "Products");

            migrationBuilder.AddColumn<int>(
                name: "FkProductId",
                table: "ProductImages",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FkProductId",
                table: "ProductImages");

            migrationBuilder.AddColumn<int>(
                name: "FkProductImage",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
