using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Inventory.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCostBaseAndPriceBaseToProduct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "CostBase",
                schema: "Inventory",
                table: "Products",
                type: "decimal(18,4)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PriceBase",
                schema: "Inventory",
                table: "Products",
                type: "decimal(18,4)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "NewCostBase",
                schema: "Inventory",
                table: "ProductPriceHistories",
                type: "decimal(18,4)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "NewPriceBase",
                schema: "Inventory",
                table: "ProductPriceHistories",
                type: "decimal(18,4)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "OldCostBase",
                schema: "Inventory",
                table: "ProductPriceHistories",
                type: "decimal(18,4)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "OldPriceBase",
                schema: "Inventory",
                table: "ProductPriceHistories",
                type: "decimal(18,4)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CostBase",
                schema: "Inventory",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "PriceBase",
                schema: "Inventory",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "NewCostBase",
                schema: "Inventory",
                table: "ProductPriceHistories");

            migrationBuilder.DropColumn(
                name: "NewPriceBase",
                schema: "Inventory",
                table: "ProductPriceHistories");

            migrationBuilder.DropColumn(
                name: "OldCostBase",
                schema: "Inventory",
                table: "ProductPriceHistories");

            migrationBuilder.DropColumn(
                name: "OldPriceBase",
                schema: "Inventory",
                table: "ProductPriceHistories");
        }
    }
}
