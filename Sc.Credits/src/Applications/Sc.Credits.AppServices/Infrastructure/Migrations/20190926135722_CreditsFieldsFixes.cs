using Microsoft.EntityFrameworkCore.Migrations;

namespace Sc.Credits.Applications.AppServices.Infrastructure.Migrations
{
    public partial class CreditsFieldsFixes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "InterestRate",
                table: "Credits",
                type: "decimal(37, 17)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(26, 6)");

            migrationBuilder.AddColumn<decimal>(
                name: "AssuranceTotalFeeValue",
                table: "Credits",
                type: "decimal(26, 6)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalDownPayment",
                table: "Credits",
                type: "decimal(26, 6)",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AssuranceTotalFeeValue",
                table: "Credits");

            migrationBuilder.DropColumn(
                name: "TotalDownPayment",
                table: "Credits");

            migrationBuilder.AlterColumn<decimal>(
                name: "InterestRate",
                table: "Credits",
                type: "decimal(26, 6)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(37, 17)");
        }
    }
}
