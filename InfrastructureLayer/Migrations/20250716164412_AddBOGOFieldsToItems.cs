using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Souq.Migrations
{
    /// <inheritdoc />
    public partial class AddBOGOFieldsToItems : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsBOGOBuy",
                table: "WashingMachines",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsBOGOGet",
                table: "WashingMachines",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsBOGOBuy",
                table: "VideoGames",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsBOGOGet",
                table: "VideoGames",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsBOGOBuy",
                table: "TVs",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsBOGOGet",
                table: "TVs",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsBOGOBuy",
                table: "MobilePhones",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsBOGOGet",
                table: "MobilePhones",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsBOGOBuy",
                table: "Laptops",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsBOGOGet",
                table: "Laptops",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsBOGOBuy",
                table: "HeadPhones",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsBOGOGet",
                table: "HeadPhones",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsBOGOBuy",
                table: "Fridges",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsBOGOGet",
                table: "Fridges",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsBOGOBuy",
                table: "Cookers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsBOGOGet",
                table: "Cookers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsBOGOBuy",
                table: "AirConditioners",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsBOGOGet",
                table: "AirConditioners",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsBOGOBuy",
                table: "WashingMachines");

            migrationBuilder.DropColumn(
                name: "IsBOGOGet",
                table: "WashingMachines");

            migrationBuilder.DropColumn(
                name: "IsBOGOBuy",
                table: "VideoGames");

            migrationBuilder.DropColumn(
                name: "IsBOGOGet",
                table: "VideoGames");

            migrationBuilder.DropColumn(
                name: "IsBOGOBuy",
                table: "TVs");

            migrationBuilder.DropColumn(
                name: "IsBOGOGet",
                table: "TVs");

            migrationBuilder.DropColumn(
                name: "IsBOGOBuy",
                table: "MobilePhones");

            migrationBuilder.DropColumn(
                name: "IsBOGOGet",
                table: "MobilePhones");

            migrationBuilder.DropColumn(
                name: "IsBOGOBuy",
                table: "Laptops");

            migrationBuilder.DropColumn(
                name: "IsBOGOGet",
                table: "Laptops");

            migrationBuilder.DropColumn(
                name: "IsBOGOBuy",
                table: "HeadPhones");

            migrationBuilder.DropColumn(
                name: "IsBOGOGet",
                table: "HeadPhones");

            migrationBuilder.DropColumn(
                name: "IsBOGOBuy",
                table: "Fridges");

            migrationBuilder.DropColumn(
                name: "IsBOGOGet",
                table: "Fridges");

            migrationBuilder.DropColumn(
                name: "IsBOGOBuy",
                table: "Cookers");

            migrationBuilder.DropColumn(
                name: "IsBOGOGet",
                table: "Cookers");

            migrationBuilder.DropColumn(
                name: "IsBOGOBuy",
                table: "AirConditioners");

            migrationBuilder.DropColumn(
                name: "IsBOGOGet",
                table: "AirConditioners");
        }
    }
}
