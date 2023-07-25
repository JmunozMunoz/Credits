using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Sc.Credits.Applications.AppServices.infrastructure.migrations
{
    public partial class AddNamePropertiesInCustomer : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "Customers",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SecondName",
                table: "Customers",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FirstLastName",
                table: "Customers",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SecondLastName",
                table: "Customers",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "MessageErrorLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Date = table.Column<DateTime>(type: "datetime", nullable: false),
                    Name = table.Column<string>(maxLength: 200, nullable: false),
                    Key = table.Column<string>(maxLength: 200, nullable: false),
                    ResourceName = table.Column<string>(maxLength: 200, nullable: false),
                    ResourceType = table.Column<int>(maxLength: 50, nullable: false),
                    Json = table.Column<string>(type: "nvarchar(MAX)", nullable: false),
                    Processed = table.Column<bool>(nullable: false),
                    Attempts = table.Column<int>(nullable: false),
                    LastAttemptDate = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessageErrorLogs", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MessageErrorLogs_Key",
                table: "MessageErrorLogs",
                column: "Key");

            migrationBuilder.CreateIndex(
                name: "IX_MessageErrorLogs_Name",
                table: "MessageErrorLogs",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_MessageErrorLogs_LastAttemptDate_Attempts_Processed",
                table: "MessageErrorLogs",
                columns: new[] { "LastAttemptDate", "Attempts", "Processed" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MessageErrorLogs");

            migrationBuilder.DropColumn(
                name: "FirstLastName",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "SecondLastName",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "SecondName",
                table: "Customers");
        }
    }
}