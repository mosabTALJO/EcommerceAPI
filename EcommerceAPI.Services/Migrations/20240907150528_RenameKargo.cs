using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace EcommerceAPI.Services.Migrations
{
    /// <inheritdoc />
    public partial class RenameKargo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "KargoCompanies");

            migrationBuilder.CreateTable(
                name: "ShippmentCompanies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NameEn = table.Column<string>(type: "text", nullable: false),
                    NameAr = table.Column<string>(type: "text", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    DeliveryLimitAmount = table.Column<double>(type: "double precision", nullable: false),
                    DeliveryLimitFixedAmount = table.Column<double>(type: "double precision", nullable: false),
                    DeliveryLimitPercent = table.Column<double>(type: "double precision", nullable: false),
                    AverageDeliveryDays = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShippmentCompanies", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ShippmentCompanies");

            migrationBuilder.CreateTable(
                name: "KargoCompanies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AverageDeliveryDays = table.Column<int>(type: "integer", nullable: false),
                    DeliveryLimitAmount = table.Column<double>(type: "double precision", nullable: false),
                    DeliveryLimitFixedAmount = table.Column<double>(type: "double precision", nullable: false),
                    DeliveryLimitPercent = table.Column<double>(type: "double precision", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    NameAr = table.Column<string>(type: "text", nullable: false),
                    NameEn = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KargoCompanies", x => x.Id);
                });
        }
    }
}
