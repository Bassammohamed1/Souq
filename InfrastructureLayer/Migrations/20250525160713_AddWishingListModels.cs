using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Souq.Migrations
{
    /// <inheritdoc />
    public partial class AddWishingListModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateSequence(
                name: "ItemSequence",
                startValue: 594,
                incrementBy: 1);

            migrationBuilder.AlterColumn<int>(
                name: "ID",
                table: "WashingMachines",
                type: "int",
                nullable: false,
                defaultValueSql: "NEXT VALUE FOR [ItemSequence]",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "ID",
                table: "VideoGames",
                type: "int",
                nullable: false,
                defaultValueSql: "NEXT VALUE FOR [ItemSequence]",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "ID",
                table: "TVs",
                type: "int",
                nullable: false,
                defaultValueSql: "NEXT VALUE FOR [ItemSequence]",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "ID",
                table: "MobilePhones",
                type: "int",
                nullable: false,
                defaultValueSql: "NEXT VALUE FOR [ItemSequence]",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "ID",
                table: "Laptops",
                type: "int",
                nullable: false,
                defaultValueSql: "NEXT VALUE FOR [ItemSequence]",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "ID",
                table: "HeadPhones",
                type: "int",
                nullable: false,
                defaultValueSql: "NEXT VALUE FOR [ItemSequence]",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "ID",
                table: "Fridges",
                type: "int",
                nullable: false,
                defaultValueSql: "NEXT VALUE FOR [ItemSequence]",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "ID",
                table: "Cookers",
                type: "int",
                nullable: false,
                defaultValueSql: "NEXT VALUE FOR [ItemSequence]",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "ID",
                table: "AirConditioners",
                type: "int",
                nullable: false,
                defaultValueSql: "NEXT VALUE FOR [ItemSequence]",
                oldClrType: typeof(int),
                oldType: "int");
                
            migrationBuilder.CreateTable(
                name: "WishingLists",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WishingLists", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WishingLists_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WishingListsDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ItemType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ItemId = table.Column<int>(type: "int", nullable: false),
                    WishingListId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WishingListsDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WishingListsDetails_WishingLists_WishingListId",
                        column: x => x.WishingListId,
                        principalTable: "WishingLists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WishingLists_UserId",
                table: "WishingLists",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WishingListsDetails_ItemId",
                table: "WishingListsDetails",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_WishingListsDetails_WishingListId",
                table: "WishingListsDetails",
                column: "WishingListId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WishingListsDetails");

            migrationBuilder.DropTable(
                name: "WishingLists");

            migrationBuilder.DropSequence(
                name: "ItemSequence");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "WashingMachines",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValueSql: "NEXT VALUE FOR [ItemSequence]")
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "VideoGames",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValueSql: "NEXT VALUE FOR [ItemSequence]")
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "TVs",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValueSql: "NEXT VALUE FOR [ItemSequence]")
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "MobilePhones",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValueSql: "NEXT VALUE FOR [ItemSequence]")
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Laptops",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValueSql: "NEXT VALUE FOR [ItemSequence]")
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "HeadPhones",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValueSql: "NEXT VALUE FOR [ItemSequence]")
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Fridges",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValueSql: "NEXT VALUE FOR [ItemSequence]")
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Cookers",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValueSql: "NEXT VALUE FOR [ItemSequence]")
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "AirConditioners",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValueSql: "NEXT VALUE FOR [ItemSequence]")
                .Annotation("SqlServer:Identity", "1, 1");
        }
    }
}
