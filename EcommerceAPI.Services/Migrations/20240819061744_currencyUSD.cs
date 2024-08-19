using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EcommerceAPI.Services.Migrations
{
    /// <inheritdoc />
    public partial class currencyUSD : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "SalesPriceUSD",
                table: "ProductVariant",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "SalesPriceUSD",
                table: "Products",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SalesPriceUSD",
                table: "ProductVariant");

            migrationBuilder.DropColumn(
                name: "SalesPriceUSD",
                table: "Products");
        }
    }
}
