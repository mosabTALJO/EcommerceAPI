using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EcommerceAPI.Services.Migrations
{
    /// <inheritdoc />
    public partial class CityCode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "City",
                table: "UserAddress");

            migrationBuilder.DropColumn(
                name: "Country",
                table: "UserAddress");

            migrationBuilder.DropColumn(
                name: "Code",
                table: "Cities");

            migrationBuilder.AddColumn<int>(
                name: "CityId",
                table: "UserAddress",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CountryId",
                table: "UserAddress",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserAddress_CityId",
                table: "UserAddress",
                column: "CityId");

            migrationBuilder.CreateIndex(
                name: "IX_UserAddress_CountryId",
                table: "UserAddress",
                column: "CountryId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserAddress_Cities_CityId",
                table: "UserAddress",
                column: "CityId",
                principalTable: "Cities",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserAddress_Countries_CountryId",
                table: "UserAddress",
                column: "CountryId",
                principalTable: "Countries",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserAddress_Cities_CityId",
                table: "UserAddress");

            migrationBuilder.DropForeignKey(
                name: "FK_UserAddress_Countries_CountryId",
                table: "UserAddress");

            migrationBuilder.DropIndex(
                name: "IX_UserAddress_CityId",
                table: "UserAddress");

            migrationBuilder.DropIndex(
                name: "IX_UserAddress_CountryId",
                table: "UserAddress");

            migrationBuilder.DropColumn(
                name: "CityId",
                table: "UserAddress");

            migrationBuilder.DropColumn(
                name: "CountryId",
                table: "UserAddress");

            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "UserAddress",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Country",
                table: "UserAddress",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "Cities",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");
        }
    }
}
