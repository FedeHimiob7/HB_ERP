using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Identity.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class changeSchemma : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Identity");

            migrationBuilder.RenameTable(
                name: "UserRoles",
                schema: "HB_ERP",
                newName: "UserRoles",
                newSchema: "Identity");

            migrationBuilder.RenameTable(
                name: "User",
                schema: "HB_ERP",
                newName: "User",
                newSchema: "Identity");

            migrationBuilder.RenameTable(
                name: "SystemActions",
                schema: "HB_ERP",
                newName: "SystemActions",
                newSchema: "Identity");

            migrationBuilder.RenameTable(
                name: "Roles",
                schema: "HB_ERP",
                newName: "Roles",
                newSchema: "Identity");

            migrationBuilder.RenameTable(
                name: "RoleActions",
                schema: "HB_ERP",
                newName: "RoleActions",
                newSchema: "Identity");

            migrationBuilder.RenameTable(
                name: "OutboxState",
                schema: "HB_ERP",
                newName: "OutboxState",
                newSchema: "Identity");

            migrationBuilder.RenameTable(
                name: "OutboxMessage",
                schema: "HB_ERP",
                newName: "OutboxMessage",
                newSchema: "Identity");

            migrationBuilder.RenameTable(
                name: "InboxState",
                schema: "HB_ERP",
                newName: "InboxState",
                newSchema: "Identity");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "HB_ERP");

            migrationBuilder.RenameTable(
                name: "UserRoles",
                schema: "Identity",
                newName: "UserRoles",
                newSchema: "HB_ERP");

            migrationBuilder.RenameTable(
                name: "User",
                schema: "Identity",
                newName: "User",
                newSchema: "HB_ERP");

            migrationBuilder.RenameTable(
                name: "SystemActions",
                schema: "Identity",
                newName: "SystemActions",
                newSchema: "HB_ERP");

            migrationBuilder.RenameTable(
                name: "Roles",
                schema: "Identity",
                newName: "Roles",
                newSchema: "HB_ERP");

            migrationBuilder.RenameTable(
                name: "RoleActions",
                schema: "Identity",
                newName: "RoleActions",
                newSchema: "HB_ERP");

            migrationBuilder.RenameTable(
                name: "OutboxState",
                schema: "Identity",
                newName: "OutboxState",
                newSchema: "HB_ERP");

            migrationBuilder.RenameTable(
                name: "OutboxMessage",
                schema: "Identity",
                newName: "OutboxMessage",
                newSchema: "HB_ERP");

            migrationBuilder.RenameTable(
                name: "InboxState",
                schema: "Identity",
                newName: "InboxState",
                newSchema: "HB_ERP");
        }
    }
}
