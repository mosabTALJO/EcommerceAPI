using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EcommerceAPI.Services.Migrations
{
    /// <inheritdoc />
    public partial class CityCode1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserAddress_Cities_CityId",
                table: "UserAddress");

            migrationBuilder.DropForeignKey(
                name: "FK_UserAddress_Countries_CountryId",
                table: "UserAddress");

            migrationBuilder.AlterColumn<int>(
                name: "CountryId",
                table: "UserAddress",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CityId",
                table: "UserAddress",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_UserAddress_Cities_CityId",
                table: "UserAddress",
                column: "CityId",
                principalTable: "Cities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserAddress_Countries_CountryId",
                table: "UserAddress",
                column: "CountryId",
                principalTable: "Countries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
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

            migrationBuilder.AlterColumn<int>(
                name: "CountryId",
                table: "UserAddress",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "CityId",
                table: "UserAddress",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

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
    }
}
