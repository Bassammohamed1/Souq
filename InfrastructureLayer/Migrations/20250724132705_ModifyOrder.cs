using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Souq.Migrations
{
    /// <inheritdoc />
    public partial class ModifyOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PromoCodeDiscountType",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "PromoCodeDiscountValue",
                table: "Orders",
                type: "float",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PromoCodeDiscountType",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "PromoCodeDiscountValue",
                table: "Orders");
        }
    }
}
