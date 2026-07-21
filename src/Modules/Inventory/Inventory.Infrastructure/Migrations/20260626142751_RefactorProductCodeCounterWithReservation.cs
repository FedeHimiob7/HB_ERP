using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Inventory.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RefactorProductCodeCounterWithReservation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductCodeCounters",
                schema: "Inventory",
                table: "ProductCodeCounters");

            migrationBuilder.RenameColumn(
                name: "DailyCounter",
                schema: "Inventory",
                table: "ProductCodeCounters",
                newName: "ItemNumberByDay");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                schema: "Inventory",
                table: "ProductCodeCounters",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "GeneratedCode",
                schema: "Inventory",
                table: "ProductCodeCounters",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsConsumed",
                schema: "Inventory",
                table: "ProductCodeCounters",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductCodeCounters",
                schema: "Inventory",
                table: "ProductCodeCounters",
                columns: new[] { "PslId", "Date", "ItemNumberByDay" });

            // Marcar filas existentes como consumidas y asignarles un GeneratedCode único
            // basado en PslId + Date para no violar el índice único
            migrationBuilder.Sql(@"
                UPDATE [Inventory].[ProductCodeCounters]
                SET [IsConsumed] = 1,
                    [GeneratedCode] = CAST([PslId] AS NVARCHAR(36)) + '-' + CONVERT(NVARCHAR(8), [Date], 112) + '-' + CAST([ItemNumberByDay] AS NVARCHAR(10)),
                    [CreatedAt] = GETUTCDATE()
                WHERE [GeneratedCode] = ''");

            migrationBuilder.CreateIndex(
                name: "IX_ProductCodeCounters_GeneratedCode_PslId",
                schema: "Inventory",
                table: "ProductCodeCounters",
                columns: new[] { "GeneratedCode", "PslId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductCodeCounters",
                schema: "Inventory",
                table: "ProductCodeCounters");

            migrationBuilder.DropIndex(
                name: "IX_ProductCodeCounters_GeneratedCode_PslId",
                schema: "Inventory",
                table: "ProductCodeCounters");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                schema: "Inventory",
                table: "ProductCodeCounters");

            migrationBuilder.DropColumn(
                name: "GeneratedCode",
                schema: "Inventory",
                table: "ProductCodeCounters");

            migrationBuilder.DropColumn(
                name: "IsConsumed",
                schema: "Inventory",
                table: "ProductCodeCounters");

            migrationBuilder.RenameColumn(
                name: "ItemNumberByDay",
                schema: "Inventory",
                table: "ProductCodeCounters",
                newName: "DailyCounter");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductCodeCounters",
                schema: "Inventory",
                table: "ProductCodeCounters",
                columns: new[] { "PslId", "Date" });
        }
    }
}
