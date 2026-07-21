using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Inventory.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddProduct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProductCodeCounters",
                schema: "Inventory",
                columns: table => new
                {
                    PslId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    PslSequenceNumber = table.Column<int>(type: "int", nullable: false),
                    DailyCounter = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductCodeCounters", x => new { x.PslId, x.Date });
                });

            migrationBuilder.CreateTable(
                name: "Products",
                schema: "Inventory",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ItemNumberByDay = table.Column<int>(type: "int", nullable: false),
                    Barcode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ClientCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Name = table.Column<string>(type: "nvarchar(70)", maxLength: 70, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(700)", maxLength: 700, nullable: true),
                    ProductServiceLineId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ProductCategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ProductSubCategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ProductBrandId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsSalable = table.Column<bool>(type: "bit", nullable: false),
                    IsPurchasable = table.Column<bool>(type: "bit", nullable: false),
                    IsStored = table.Column<bool>(type: "bit", nullable: false),
                    Cost = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    CostCurrencyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CostExchangeRate = table.Column<decimal>(type: "decimal(18,6)", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    PriceCurrencyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PriceExchangeRate = table.Column<decimal>(type: "decimal(18,6)", nullable: false),
                    Price2 = table.Column<decimal>(type: "decimal(18,4)", nullable: true),
                    Price3 = table.Column<decimal>(type: "decimal(18,4)", nullable: true),
                    Price4 = table.Column<decimal>(type: "decimal(18,4)", nullable: true),
                    Price5 = table.Column<decimal>(type: "decimal(18,4)", nullable: true),
                    PurchaseUnitId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SaleUnitId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UnitConversionFactor = table.Column<decimal>(type: "decimal(18,6)", nullable: true),
                    Weight = table.Column<decimal>(type: "decimal(18,4)", nullable: true),
                    Volume = table.Column<decimal>(type: "decimal(18,4)", nullable: true),
                    ContentCapacity = table.Column<decimal>(type: "decimal(18,4)", nullable: true),
                    Tags = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ImageUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ProfitMargin = table.Column<decimal>(type: "decimal(18,4)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PurchaseTaxIds = table.Column<string>(type: "nvarchar(max)", nullable: true, defaultValue: "[]"),
                    SaleTaxIds = table.Column<string>(type: "nvarchar(max)", nullable: true, defaultValue: "[]")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProductPriceHistories",
                schema: "Inventory",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ChangedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ChangedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OldCost = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    OldCostCurrencyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OldCostExchangeRate = table.Column<decimal>(type: "decimal(18,6)", nullable: false),
                    NewCost = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    NewCostCurrencyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NewCostExchangeRate = table.Column<decimal>(type: "decimal(18,6)", nullable: false),
                    OldPrice = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    OldPriceCurrencyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OldPriceExchangeRate = table.Column<decimal>(type: "decimal(18,6)", nullable: false),
                    NewPrice = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    NewPriceCurrencyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NewPriceExchangeRate = table.Column<decimal>(type: "decimal(18,6)", nullable: false),
                    OldPrice2 = table.Column<decimal>(type: "decimal(18,4)", nullable: true),
                    NewPrice2 = table.Column<decimal>(type: "decimal(18,4)", nullable: true),
                    OldPrice3 = table.Column<decimal>(type: "decimal(18,4)", nullable: true),
                    NewPrice3 = table.Column<decimal>(type: "decimal(18,4)", nullable: true),
                    OldPrice4 = table.Column<decimal>(type: "decimal(18,4)", nullable: true),
                    NewPrice4 = table.Column<decimal>(type: "decimal(18,4)", nullable: true),
                    OldPrice5 = table.Column<decimal>(type: "decimal(18,4)", nullable: true),
                    NewPrice5 = table.Column<decimal>(type: "decimal(18,4)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductPriceHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductPriceHistories_Products_ProductId",
                        column: x => x.ProductId,
                        principalSchema: "Inventory",
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductPriceHistories_ProductId",
                schema: "Inventory",
                table: "ProductPriceHistories",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_Code_ProductServiceLineId",
                schema: "Inventory",
                table: "Products",
                columns: new[] { "Code", "ProductServiceLineId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductCodeCounters",
                schema: "Inventory");

            migrationBuilder.DropTable(
                name: "ProductPriceHistories",
                schema: "Inventory");

            migrationBuilder.DropTable(
                name: "Products",
                schema: "Inventory");
        }
    }
}
