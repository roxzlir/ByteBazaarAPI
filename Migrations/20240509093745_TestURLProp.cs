﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ByteBazaarAPI.Migrations
{
    /// <inheritdoc />
    public partial class TestURLProp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ProductURL",
                table: "Products",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProductURL",
                table: "Products");
        }
    }
}
