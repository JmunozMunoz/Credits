using Microsoft.EntityFrameworkCore.Migrations;

namespace Sc.Credits.AppServices.Infrastructure.Migrations
{
    public partial class RefactorFieldsCredits : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ArrearsChargeBalance",
                table: "Credits");

            migrationBuilder.DropColumn(
                name: "ChargeBalance",
                table: "Credits");

            migrationBuilder.AddColumn<decimal>(
                name: "ArrearsEffectiveAnnualRate",
                table: "Profiles",
                type: "decimal(26, 6)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "EffectiveAnnualRate",
                table: "CreditsMaster",
                type: "decimal(26, 6)",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ArrearsEffectiveAnnualRate",
                table: "Profiles");

            migrationBuilder.DropColumn(
                name: "EffectiveAnnualRate",
                table: "CreditsMaster");

            migrationBuilder.AddColumn<decimal>(
                name: "ArrearsChargeBalance",
                table: "Credits",
                type: "decimal(26, 6)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ChargeBalance",
                table: "Credits",
                type: "decimal(26, 6)",
                nullable: true);
        }
    }
}