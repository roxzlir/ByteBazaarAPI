using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ByteBazaarAPI.Migrations
{
    /// <inheritdoc />
    public partial class Campaign : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Percent",
                table: "Products",
                newName: "CampaignPercent");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CampaignPercent",
                table: "Products",
                newName: "Percent");
        }
    }
}
