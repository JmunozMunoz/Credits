using Microsoft.EntityFrameworkCore.Migrations;

namespace Sc.Credits.Applications.AppServices.Infrastructure.Migrations
{
    public partial class EffectiveAnnualRateAndAssurancePercentageInStores : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AssurancePercentage",
                table: "Profiles");

            migrationBuilder.DropColumn(
                name: "EffectiveAnnualRate",
                table: "Profiles");

            migrationBuilder.AddColumn<decimal>(
                name: "AssurancePercentage",
                table: "Stores",
                type: "decimal(26, 6)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "EffectiveAnnualRate",
                table: "Stores",
                type: "decimal(26, 6)",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AssurancePercentage",
                table: "Stores");

            migrationBuilder.DropColumn(
                name: "EffectiveAnnualRate",
                table: "Stores");

            migrationBuilder.AddColumn<decimal>(
                name: "AssurancePercentage",
                table: "Profiles",
                type: "decimal(26, 6)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "EffectiveAnnualRate",
                table: "Profiles",
                type: "decimal(26, 6)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
