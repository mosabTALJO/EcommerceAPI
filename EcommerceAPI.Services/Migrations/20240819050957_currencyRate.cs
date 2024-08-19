using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EcommerceAPI.Services.Migrations
{
    /// <inheritdoc />
    public partial class currencyRate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Code",
                table: "CurrencyRates");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "CurrencyRates");

            migrationBuilder.RenameColumn(
                name: "Title",
                table: "CurrencyRates",
                newName: "ToCurrencyCode");

            migrationBuilder.RenameColumn(
                name: "CountryCode",
                table: "CurrencyRates",
                newName: "FromCurrencyCode");

            migrationBuilder.AddColumn<string>(
                name: "Currency",
                table: "Products",
                type: "character varying(3)",
                maxLength: 3,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "CurrencyCode",
                table: "Countries",
                type: "character varying(3)",
                maxLength: 3,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(10)",
                oldMaxLength: 10);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Currency",
                table: "Products");

            migrationBuilder.RenameColumn(
                name: "ToCurrencyCode",
                table: "CurrencyRates",
                newName: "Title");

            migrationBuilder.RenameColumn(
                name: "FromCurrencyCode",
                table: "CurrencyRates",
                newName: "CountryCode");

            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "CurrencyRates",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "CurrencyRates",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "CurrencyCode",
                table: "Countries",
                type: "character varying(10)",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(3)",
                oldMaxLength: 3);
        }
    }
}
