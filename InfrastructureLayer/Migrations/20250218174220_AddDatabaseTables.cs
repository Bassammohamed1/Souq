using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Souq.Migrations
{
    /// <inheritdoc />
    public partial class AddDatabaseTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    dbImage = table.Column<byte[]>(type: "varbinary(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Departments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Departments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AirConditioners",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Capacity = table.Column<int>(type: "int", nullable: false),
                    CoolingPower = table.Column<int>(type: "int", nullable: false),
                    NoiseLevel = table.Column<int>(type: "int", nullable: false),
                    Voltage = table.Column<int>(type: "int", nullable: false),
                    Color = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ItemDimensions = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Price = table.Column<double>(type: "float", nullable: false),
                    Amount = table.Column<int>(type: "int", nullable: false),
                    AddedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Rate = table.Column<double>(type: "float", nullable: false),
                    dbImage = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    CategoryId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AirConditioners", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AirConditioners_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Cookers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ItemDimensions = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DrawerType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Material = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FinishType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModelName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FormFactor = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ControlsType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ItemWeight = table.Column<int>(type: "int", nullable: false),
                    NumberOfHeatingElements = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Price = table.Column<double>(type: "float", nullable: false),
                    Amount = table.Column<int>(type: "int", nullable: false),
                    AddedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Rate = table.Column<double>(type: "float", nullable: false),
                    dbImage = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    CategoryId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cookers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Cookers_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Fridges",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ItemDimensions = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Capacity = table.Column<int>(type: "int", nullable: false),
                    EnergyStar = table.Column<int>(type: "int", nullable: false),
                    NumberOfDoors = table.Column<int>(type: "int", nullable: false),
                    Color = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SpecialFeatures = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    InstallationType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DefrostSystem = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Price = table.Column<double>(type: "float", nullable: false),
                    Amount = table.Column<int>(type: "int", nullable: false),
                    AddedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Rate = table.Column<double>(type: "float", nullable: false),
                    dbImage = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    CategoryId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Fridges", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Fridges_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HeadPhones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Color = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HeadphonesEarPlacement = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HeadphonesFormFactor = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NoiseControl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModelName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ConnectivityTechnology = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Price = table.Column<double>(type: "float", nullable: false),
                    Amount = table.Column<int>(type: "int", nullable: false),
                    AddedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Rate = table.Column<double>(type: "float", nullable: false),
                    dbImage = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    CategoryId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HeadPhones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HeadPhones_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Laptops",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ScreenSize = table.Column<double>(type: "float", nullable: false),
                    ModelName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Color = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RAM = table.Column<int>(type: "int", nullable: false),
                    HardDiskSize = table.Column<int>(type: "int", nullable: false),
                    CPU = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GPU = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OperatingSystem = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HardDiskDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Price = table.Column<double>(type: "float", nullable: false),
                    Amount = table.Column<int>(type: "int", nullable: false),
                    AddedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Rate = table.Column<double>(type: "float", nullable: false),
                    dbImage = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    CategoryId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Laptops", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Laptops_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MobilePhones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Color = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RAM = table.Column<int>(type: "int", nullable: false),
                    MemoryStorageCapacity = table.Column<int>(type: "int", nullable: false),
                    OperatingSystem = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CPUModel = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Price = table.Column<double>(type: "float", nullable: false),
                    Amount = table.Column<int>(type: "int", nullable: false),
                    AddedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Rate = table.Column<double>(type: "float", nullable: false),
                    dbImage = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    CategoryId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MobilePhones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MobilePhones_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TVs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ScreenSize = table.Column<double>(type: "float", nullable: false),
                    RefreshRate = table.Column<int>(type: "int", nullable: false),
                    Resolution = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DisplayTechnology = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SpecialFeatures = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Price = table.Column<double>(type: "float", nullable: false),
                    Amount = table.Column<int>(type: "int", nullable: false),
                    AddedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Rate = table.Column<double>(type: "float", nullable: false),
                    dbImage = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    CategoryId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TVs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TVs_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VideoGames",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Price = table.Column<double>(type: "float", nullable: false),
                    Amount = table.Column<int>(type: "int", nullable: false),
                    AddedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Rate = table.Column<double>(type: "float", nullable: false),
                    dbImage = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    CategoryId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VideoGames", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VideoGames_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WashingMachines",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ItemDimensions = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CycleOptions = table.Column<int>(type: "int", nullable: false),
                    ItemWeight = table.Column<int>(type: "int", nullable: false),
                    Capacity = table.Column<int>(type: "int", nullable: false),
                    SpecialFeatures = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Color = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Price = table.Column<double>(type: "float", nullable: false),
                    Amount = table.Column<int>(type: "int", nullable: false),
                    AddedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Rate = table.Column<double>(type: "float", nullable: false),
                    dbImage = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    CategoryId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WashingMachines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WashingMachines_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CategoryDepartments",
                columns: table => new
                {
                    CategoryId = table.Column<int>(type: "int", nullable: false),
                    DepartmentId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoryDepartments", x => new { x.DepartmentId, x.CategoryId });
                    table.ForeignKey(
                        name: "FK_CategoryDepartments_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CategoryDepartments_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AirConditioners_CategoryId",
                table: "AirConditioners",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_CategoryDepartments_CategoryId",
                table: "CategoryDepartments",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Cookers_CategoryId",
                table: "Cookers",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Fridges_CategoryId",
                table: "Fridges",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_HeadPhones_CategoryId",
                table: "HeadPhones",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Laptops_CategoryId",
                table: "Laptops",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_MobilePhones_CategoryId",
                table: "MobilePhones",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_TVs_CategoryId",
                table: "TVs",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_VideoGames_CategoryId",
                table: "VideoGames",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_WashingMachines_CategoryId",
                table: "WashingMachines",
                column: "CategoryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AirConditioners");

            migrationBuilder.DropTable(
                name: "CategoryDepartments");

            migrationBuilder.DropTable(
                name: "Cookers");

            migrationBuilder.DropTable(
                name: "Fridges");

            migrationBuilder.DropTable(
                name: "HeadPhones");

            migrationBuilder.DropTable(
                name: "Laptops");

            migrationBuilder.DropTable(
                name: "MobilePhones");

            migrationBuilder.DropTable(
                name: "TVs");

            migrationBuilder.DropTable(
                name: "VideoGames");

            migrationBuilder.DropTable(
                name: "WashingMachines");

            migrationBuilder.DropTable(
                name: "Departments");

            migrationBuilder.DropTable(
                name: "Categories");
        }
    }
}
