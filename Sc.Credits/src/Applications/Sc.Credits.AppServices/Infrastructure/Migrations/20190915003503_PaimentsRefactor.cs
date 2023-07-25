using Microsoft.EntityFrameworkCore.Migrations;

namespace Sc.Credits.Applications.AppServices.Infrastructure.Migrations
{
    public partial class PaimentsRefactor : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ArrearsEffectiveAnnualRate",
                table: "Profiles");

            migrationBuilder.DropColumn(
                name: "UpdatedPaymentPlanBalance",
                table: "Credits");

            migrationBuilder.DropColumn(
                name: "Fee",
                table: "Credits");

            migrationBuilder.AddColumn<decimal>(
                name: "DownPayment",
                table: "Credits",
                type: "decimal(26, 6)",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DownPayment",
                table: "Credits");

            migrationBuilder.AddColumn<decimal>(
                name: "ArrearsEffectiveAnnualRate",
                table: "Profiles",
                type: "decimal(26, 6)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "UpdatedPaymentPlanBalance",
                table: "Credits",
                type: "decimal(26, 6)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Fee",
                table: "Credits",
                nullable: false,
                defaultValue: 0);
        }
    }
}
