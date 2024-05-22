using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ByteBazaarAPI.Migrations
{
    /// <inheritdoc />
    public partial class modelUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ISCampaign",
                table: "Products",
                newName: "IsCampaign");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsCampaign",
                table: "Products",
                newName: "ISCampaign");
        }
    }
}
