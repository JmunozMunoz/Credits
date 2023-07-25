using Microsoft.EntityFrameworkCore.Migrations;

namespace Sc.Credits.Applications.AppServices.Infrastructure.Migrations
{
    public partial class _8186_PaymentApplyFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "ActiveFeeValuePaid",
                table: "Credits",
                type: "decimal(26, 6)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PreviousArrears",
                table: "Credits",
                type: "decimal(26, 6)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PreviousInterest",
                table: "Credits",
                type: "decimal(26, 6)",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActiveFeeValuePaid",
                table: "Credits");

            migrationBuilder.DropColumn(
                name: "PreviousArrears",
                table: "Credits");

            migrationBuilder.DropColumn(
                name: "PreviousInterest",
                table: "Credits");
        }
    }
}