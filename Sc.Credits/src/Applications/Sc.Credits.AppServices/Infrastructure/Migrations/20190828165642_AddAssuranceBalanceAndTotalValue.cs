using Microsoft.EntityFrameworkCore.Migrations;

namespace Sc.Credits.AppServices.Infrastructure.Migrations
{
    public partial class AddAssuranceBalanceAndTotalValue : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "AssuranceBalance",
                table: "Credits",
                type: "decimal(26, 6)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "AssuranceTotalValue",
                table: "Credits",
                type: "decimal(26, 6)",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AssuranceBalance",
                table: "Credits");

            migrationBuilder.DropColumn(
                name: "AssuranceTotalValue",
                table: "Credits");
        }
    }
}