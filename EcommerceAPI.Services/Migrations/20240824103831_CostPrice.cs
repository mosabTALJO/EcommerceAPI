using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EcommerceAPI.Services.Migrations
{
    /// <inheritdoc />
    public partial class CostPrice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "DefaultCostPrice",
                table: "Products",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "CostPrice",
                table: "OrderItem",
                type: "double precision",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DefaultCostPrice",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "CostPrice",
                table: "OrderItem");
        }
    }
}
