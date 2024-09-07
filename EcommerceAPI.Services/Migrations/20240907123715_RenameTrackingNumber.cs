using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EcommerceAPI.Services.Migrations
{
    /// <inheritdoc />
    public partial class RenameTrackingNumber : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ShippingCode",
                table: "OrderItem",
                newName: "TrackingNumber");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TrackingNumber",
                table: "OrderItem",
                newName: "ShippingCode");
        }
    }
}
