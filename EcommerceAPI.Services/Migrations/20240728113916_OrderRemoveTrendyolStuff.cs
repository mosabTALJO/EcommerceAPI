using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EcommerceAPI.Services.Migrations
{
    /// <inheritdoc />
    public partial class OrderRemoveTrendyolStuff : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "KDV",
                table: "OrderItem");

            migrationBuilder.DropColumn(
                name: "OrderNumber",
                table: "OrderItem");

            migrationBuilder.DropColumn(
                name: "ProductUrl",
                table: "OrderItem");

            migrationBuilder.DropColumn(
                name: "SalePriceNoKDV",
                table: "OrderItem");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "KDV",
                table: "OrderItem",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "OrderNumber",
                table: "OrderItem",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ProductUrl",
                table: "OrderItem",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<double>(
                name: "SalePriceNoKDV",
                table: "OrderItem",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);
        }
    }
}
