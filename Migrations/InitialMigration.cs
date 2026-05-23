// ────────────────────────────────────────────────────────────────────
// Этот файл генерируется командой:  dotnet ef migrations add Initial
// Здесь показан ШАБЛОН для ручного включения в проект.
// Для реального использования выполните в терминале:
//   dotnet ef migrations add Initial
//   dotnet ef database update
// ────────────────────────────────────────────────────────────────────
using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InventoryApp.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categories",
                columns: t => new
                {
                    Id   = t.Column<int>(nullable: false).Annotation("Sqlite:Autoincrement", true),
                    Name = t.Column<string>(nullable: false)
                },
                constraints: t => t.PrimaryKey("PK_Categories", x => x.Id));

            migrationBuilder.CreateTable(
                name: "Users",
                columns: t => new
                {
                    Id   = t.Column<int>(nullable: false).Annotation("Sqlite:Autoincrement", true),
                    Name = t.Column<string>(nullable: false)
                },
                constraints: t => t.PrimaryKey("PK_Users", x => x.Id));

            migrationBuilder.CreateTable(
                name: "Products",
                columns: t => new
                {
                    Id         = t.Column<int>(nullable: false).Annotation("Sqlite:Autoincrement", true),
                    Name       = t.Column<string>(nullable: false),
                    SKU        = t.Column<string>(nullable: false),
                    Quantity   = t.Column<int>(nullable: false),
                    CategoryId = t.Column<int>(nullable: false)
                },
                constraints: t =>
                {
                    t.PrimaryKey("PK_Products", x => x.Id);
                    t.ForeignKey("FK_Products_Categories_CategoryId",
                        x => x.CategoryId, "Categories", "Id", onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StockMovements",
                columns: t => new
                {
                    Id        = t.Column<int>(nullable: false).Annotation("Sqlite:Autoincrement", true),
                    ProductId = t.Column<int>(nullable: false),
                    Type      = t.Column<int>(nullable: false),
                    Quantity  = t.Column<int>(nullable: false),
                    Date      = t.Column<DateTime>(nullable: false),
                    Comment   = t.Column<string>(nullable: false)
                },
                constraints: t =>
                {
                    t.PrimaryKey("PK_StockMovements", x => x.Id);
                    t.ForeignKey("FK_StockMovements_Products_ProductId",
                        x => x.ProductId, "Products", "Id", onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex("IX_Products_SKU", "Products", "SKU", unique: true);

            // Seed categories
            migrationBuilder.InsertData("Categories", new[] { "Id", "Name" },
                new object[] { 1, "Электроника" });
            migrationBuilder.InsertData("Categories", new[] { "Id", "Name" },
                new object[] { 2, "Канцелярия" });
            migrationBuilder.InsertData("Categories", new[] { "Id", "Name" },
                new object[] { 3, "Прочее" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable("StockMovements");
            migrationBuilder.DropTable("Products");
            migrationBuilder.DropTable("Categories");
            migrationBuilder.DropTable("Users");
        }
    }
}
