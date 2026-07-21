using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Inventory.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MakeProductPricesOptional : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Products — bloque costo
            migrationBuilder.AlterColumn<decimal>(
                name: "Cost",
                schema: "Inventory",
                table: "Products",
                type: "decimal(18,4)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,4)");

            migrationBuilder.AlterColumn<Guid>(
                name: "CostCurrencyId",
                schema: "Inventory",
                table: "Products",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<decimal>(
                name: "CostExchangeRate",
                schema: "Inventory",
                table: "Products",
                type: "decimal(18,6)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,6)");

            // Products — bloque precio
            migrationBuilder.AlterColumn<decimal>(
                name: "Price",
                schema: "Inventory",
                table: "Products",
                type: "decimal(18,4)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,4)");

            migrationBuilder.AlterColumn<Guid>(
                name: "PriceCurrencyId",
                schema: "Inventory",
                table: "Products",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<decimal>(
                name: "PriceExchangeRate",
                schema: "Inventory",
                table: "Products",
                type: "decimal(18,6)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,6)");

            // ProductPriceHistories — bloque costo viejo
            migrationBuilder.AlterColumn<decimal>(
                name: "OldCost",
                schema: "Inventory",
                table: "ProductPriceHistories",
                type: "decimal(18,4)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,4)");

            migrationBuilder.AlterColumn<Guid>(
                name: "OldCostCurrencyId",
                schema: "Inventory",
                table: "ProductPriceHistories",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<decimal>(
                name: "OldCostExchangeRate",
                schema: "Inventory",
                table: "ProductPriceHistories",
                type: "decimal(18,6)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,6)");

            // ProductPriceHistories — bloque costo nuevo
            migrationBuilder.AlterColumn<decimal>(
                name: "NewCost",
                schema: "Inventory",
                table: "ProductPriceHistories",
                type: "decimal(18,4)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,4)");

            migrationBuilder.AlterColumn<Guid>(
                name: "NewCostCurrencyId",
                schema: "Inventory",
                table: "ProductPriceHistories",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<decimal>(
                name: "NewCostExchangeRate",
                schema: "Inventory",
                table: "ProductPriceHistories",
                type: "decimal(18,6)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,6)");

            // ProductPriceHistories — bloque precio viejo
            migrationBuilder.AlterColumn<decimal>(
                name: "OldPrice",
                schema: "Inventory",
                table: "ProductPriceHistories",
                type: "decimal(18,4)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,4)");

            migrationBuilder.AlterColumn<Guid>(
                name: "OldPriceCurrencyId",
                schema: "Inventory",
                table: "ProductPriceHistories",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<decimal>(
                name: "OldPriceExchangeRate",
                schema: "Inventory",
                table: "ProductPriceHistories",
                type: "decimal(18,6)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,6)");

            // ProductPriceHistories — bloque precio nuevo
            migrationBuilder.AlterColumn<decimal>(
                name: "NewPrice",
                schema: "Inventory",
                table: "ProductPriceHistories",
                type: "decimal(18,4)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,4)");

            migrationBuilder.AlterColumn<Guid>(
                name: "NewPriceCurrencyId",
                schema: "Inventory",
                table: "ProductPriceHistories",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<decimal>(
                name: "NewPriceExchangeRate",
                schema: "Inventory",
                table: "ProductPriceHistories",
                type: "decimal(18,6)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,6)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(name: "Cost", schema: "Inventory", table: "Products",
                type: "decimal(18,4)", nullable: false, oldClrType: typeof(decimal), oldType: "decimal(18,4)", oldNullable: true);
            migrationBuilder.AlterColumn<Guid>(name: "CostCurrencyId", schema: "Inventory", table: "Products",
                type: "uniqueidentifier", nullable: false, oldClrType: typeof(Guid), oldType: "uniqueidentifier", oldNullable: true);
            migrationBuilder.AlterColumn<decimal>(name: "CostExchangeRate", schema: "Inventory", table: "Products",
                type: "decimal(18,6)", nullable: false, oldClrType: typeof(decimal), oldType: "decimal(18,6)", oldNullable: true);
            migrationBuilder.AlterColumn<decimal>(name: "Price", schema: "Inventory", table: "Products",
                type: "decimal(18,4)", nullable: false, oldClrType: typeof(decimal), oldType: "decimal(18,4)", oldNullable: true);
            migrationBuilder.AlterColumn<Guid>(name: "PriceCurrencyId", schema: "Inventory", table: "Products",
                type: "uniqueidentifier", nullable: false, oldClrType: typeof(Guid), oldType: "uniqueidentifier", oldNullable: true);
            migrationBuilder.AlterColumn<decimal>(name: "PriceExchangeRate", schema: "Inventory", table: "Products",
                type: "decimal(18,6)", nullable: false, oldClrType: typeof(decimal), oldType: "decimal(18,6)", oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(name: "OldCost", schema: "Inventory", table: "ProductPriceHistories",
                type: "decimal(18,4)", nullable: false, oldClrType: typeof(decimal), oldType: "decimal(18,4)", oldNullable: true);
            migrationBuilder.AlterColumn<Guid>(name: "OldCostCurrencyId", schema: "Inventory", table: "ProductPriceHistories",
                type: "uniqueidentifier", nullable: false, oldClrType: typeof(Guid), oldType: "uniqueidentifier", oldNullable: true);
            migrationBuilder.AlterColumn<decimal>(name: "OldCostExchangeRate", schema: "Inventory", table: "ProductPriceHistories",
                type: "decimal(18,6)", nullable: false, oldClrType: typeof(decimal), oldType: "decimal(18,6)", oldNullable: true);
            migrationBuilder.AlterColumn<decimal>(name: "NewCost", schema: "Inventory", table: "ProductPriceHistories",
                type: "decimal(18,4)", nullable: false, oldClrType: typeof(decimal), oldType: "decimal(18,4)", oldNullable: true);
            migrationBuilder.AlterColumn<Guid>(name: "NewCostCurrencyId", schema: "Inventory", table: "ProductPriceHistories",
                type: "uniqueidentifier", nullable: false, oldClrType: typeof(Guid), oldType: "uniqueidentifier", oldNullable: true);
            migrationBuilder.AlterColumn<decimal>(name: "NewCostExchangeRate", schema: "Inventory", table: "ProductPriceHistories",
                type: "decimal(18,6)", nullable: false, oldClrType: typeof(decimal), oldType: "decimal(18,6)", oldNullable: true);
            migrationBuilder.AlterColumn<decimal>(name: "OldPrice", schema: "Inventory", table: "ProductPriceHistories",
                type: "decimal(18,4)", nullable: false, oldClrType: typeof(decimal), oldType: "decimal(18,4)", oldNullable: true);
            migrationBuilder.AlterColumn<Guid>(name: "OldPriceCurrencyId", schema: "Inventory", table: "ProductPriceHistories",
                type: "uniqueidentifier", nullable: false, oldClrType: typeof(Guid), oldType: "uniqueidentifier", oldNullable: true);
            migrationBuilder.AlterColumn<decimal>(name: "OldPriceExchangeRate", schema: "Inventory", table: "ProductPriceHistories",
                type: "decimal(18,6)", nullable: false, oldClrType: typeof(decimal), oldType: "decimal(18,6)", oldNullable: true);
            migrationBuilder.AlterColumn<decimal>(name: "NewPrice", schema: "Inventory", table: "ProductPriceHistories",
                type: "decimal(18,4)", nullable: false, oldClrType: typeof(decimal), oldType: "decimal(18,4)", oldNullable: true);
            migrationBuilder.AlterColumn<Guid>(name: "NewPriceCurrencyId", schema: "Inventory", table: "ProductPriceHistories",
                type: "uniqueidentifier", nullable: false, oldClrType: typeof(Guid), oldType: "uniqueidentifier", oldNullable: true);
            migrationBuilder.AlterColumn<decimal>(name: "NewPriceExchangeRate", schema: "Inventory", table: "ProductPriceHistories",
                type: "decimal(18,6)", nullable: false, oldClrType: typeof(decimal), oldType: "decimal(18,6)", oldNullable: true);
        }
    }
}
