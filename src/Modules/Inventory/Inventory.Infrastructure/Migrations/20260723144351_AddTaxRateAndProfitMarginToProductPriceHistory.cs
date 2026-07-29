using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Inventory.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTaxRateAndProfitMarginToProductPriceHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "NewProfitMargin",
                schema: "Inventory",
                table: "ProductPriceHistories",
                type: "decimal(18,4)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "NewPurchaseTaxRate",
                schema: "Inventory",
                table: "ProductPriceHistories",
                type: "decimal(9,4)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "NewSaleTaxRate",
                schema: "Inventory",
                table: "ProductPriceHistories",
                type: "decimal(9,4)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "OldProfitMargin",
                schema: "Inventory",
                table: "ProductPriceHistories",
                type: "decimal(18,4)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "OldPurchaseTaxRate",
                schema: "Inventory",
                table: "ProductPriceHistories",
                type: "decimal(9,4)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "OldSaleTaxRate",
                schema: "Inventory",
                table: "ProductPriceHistories",
                type: "decimal(9,4)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NewProfitMargin",
                schema: "Inventory",
                table: "ProductPriceHistories");

            migrationBuilder.DropColumn(
                name: "NewPurchaseTaxRate",
                schema: "Inventory",
                table: "ProductPriceHistories");

            migrationBuilder.DropColumn(
                name: "NewSaleTaxRate",
                schema: "Inventory",
                table: "ProductPriceHistories");

            migrationBuilder.DropColumn(
                name: "OldProfitMargin",
                schema: "Inventory",
                table: "ProductPriceHistories");

            migrationBuilder.DropColumn(
                name: "OldPurchaseTaxRate",
                schema: "Inventory",
                table: "ProductPriceHistories");

            migrationBuilder.DropColumn(
                name: "OldSaleTaxRate",
                schema: "Inventory",
                table: "ProductPriceHistories");
        }
    }
}
