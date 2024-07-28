using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EcommerceAPI.Services.Migrations
{
    /// <inheritdoc />
    public partial class OrderItemImage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MainImage",
                table: "ProductVariant");

            migrationBuilder.DropColumn(
                name: "ProductCompanyPrice",
                table: "OrderItem");

            migrationBuilder.AddColumn<string>(
                name: "ProductImage",
                table: "OrderItem",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ProductName",
                table: "OrderItem",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProductImage",
                table: "OrderItem");

            migrationBuilder.DropColumn(
                name: "ProductName",
                table: "OrderItem");

            migrationBuilder.AddColumn<string>(
                name: "MainImage",
                table: "ProductVariant",
                type: "character varying(400)",
                maxLength: 400,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<double>(
                name: "ProductCompanyPrice",
                table: "OrderItem",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);
        }
    }
}
