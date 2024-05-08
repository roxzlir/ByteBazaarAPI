using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ByteBazaarAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddedFkKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Products_FKCategoryId",
                table: "Products",
                column: "FkCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Categoryes_FkCategoryId",
                table: "Products",
                column: "FkCategoryId",
                principalTable: "Categories",
                principalColumn: "CategoryId",
                onDelete: ReferentialAction.Cascade
                );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
