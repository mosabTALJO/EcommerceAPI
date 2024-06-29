using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EcommerceAPI.Services.Migrations
{
    /// <inheritdoc />
    public partial class updateAttribute : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "ProductAttribute");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "ProductAttribute",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "ProductAttribute",
                type: "character varying(40)",
                maxLength: 40,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "ProductAttribute",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "ProductAttribute",
                type: "character varying(40)",
                maxLength: 40,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NameAr",
                table: "ProductAttribute",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NameEn",
                table: "ProductAttribute",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NameTr",
                table: "ProductAttribute",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "ProductAttribute",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "ProductAttribute",
                type: "character varying(40)",
                maxLength: 40,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "ProductAttribute");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "ProductAttribute");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "ProductAttribute");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "ProductAttribute");

            migrationBuilder.DropColumn(
                name: "NameAr",
                table: "ProductAttribute");

            migrationBuilder.DropColumn(
                name: "NameEn",
                table: "ProductAttribute");

            migrationBuilder.DropColumn(
                name: "NameTr",
                table: "ProductAttribute");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "ProductAttribute");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "ProductAttribute");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "ProductAttribute",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
