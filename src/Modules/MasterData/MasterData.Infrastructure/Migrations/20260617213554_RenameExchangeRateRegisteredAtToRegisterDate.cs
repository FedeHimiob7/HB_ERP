using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MasterData.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RenameExchangeRateRegisteredAtToRegisterDate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RegisteredAt",
                schema: "MasterData",
                table: "ExchangeRates",
                newName: "RegisterDate");

            migrationBuilder.RenameIndex(
                name: "IX_ExchangeRates_RegisteredAt",
                schema: "MasterData",
                table: "ExchangeRates",
                newName: "IX_ExchangeRates_RegisterDate");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RegisterDate",
                schema: "MasterData",
                table: "ExchangeRates",
                newName: "RegisteredAt");

            migrationBuilder.RenameIndex(
                name: "IX_ExchangeRates_RegisterDate",
                schema: "MasterData",
                table: "ExchangeRates",
                newName: "IX_ExchangeRates_RegisteredAt");
        }
    }
}
