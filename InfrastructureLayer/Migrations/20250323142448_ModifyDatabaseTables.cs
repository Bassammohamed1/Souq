using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Souq.Migrations
{
    /// <inheritdoc />
    public partial class ModifyDatabaseTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDiscounted",
                table: "WashingMachines",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "NewPrice",
                table: "WashingMachines",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDiscounted",
                table: "VideoGames",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "NewPrice",
                table: "VideoGames",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDiscounted",
                table: "TVs",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "NewPrice",
                table: "TVs",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDiscounted",
                table: "MobilePhones",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "NewPrice",
                table: "MobilePhones",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDiscounted",
                table: "Laptops",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "NewPrice",
                table: "Laptops",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDiscounted",
                table: "HeadPhones",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "NewPrice",
                table: "HeadPhones",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDiscounted",
                table: "Fridges",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "NewPrice",
                table: "Fridges",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDiscounted",
                table: "Cookers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "NewPrice",
                table: "Cookers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDiscounted",
                table: "AirConditioners",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "NewPrice",
                table: "AirConditioners",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDiscounted",
                table: "WashingMachines");

            migrationBuilder.DropColumn(
                name: "NewPrice",
                table: "WashingMachines");

            migrationBuilder.DropColumn(
                name: "IsDiscounted",
                table: "VideoGames");

            migrationBuilder.DropColumn(
                name: "NewPrice",
                table: "VideoGames");

            migrationBuilder.DropColumn(
                name: "IsDiscounted",
                table: "TVs");

            migrationBuilder.DropColumn(
                name: "NewPrice",
                table: "TVs");

            migrationBuilder.DropColumn(
                name: "IsDiscounted",
                table: "MobilePhones");

            migrationBuilder.DropColumn(
                name: "NewPrice",
                table: "MobilePhones");

            migrationBuilder.DropColumn(
                name: "IsDiscounted",
                table: "Laptops");

            migrationBuilder.DropColumn(
                name: "NewPrice",
                table: "Laptops");

            migrationBuilder.DropColumn(
                name: "IsDiscounted",
                table: "HeadPhones");

            migrationBuilder.DropColumn(
                name: "NewPrice",
                table: "HeadPhones");

            migrationBuilder.DropColumn(
                name: "IsDiscounted",
                table: "Fridges");

            migrationBuilder.DropColumn(
                name: "NewPrice",
                table: "Fridges");

            migrationBuilder.DropColumn(
                name: "IsDiscounted",
                table: "Cookers");

            migrationBuilder.DropColumn(
                name: "NewPrice",
                table: "Cookers");

            migrationBuilder.DropColumn(
                name: "IsDiscounted",
                table: "AirConditioners");

            migrationBuilder.DropColumn(
                name: "NewPrice",
                table: "AirConditioners");
        }
    }
}
