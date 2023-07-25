using Microsoft.EntityFrameworkCore.Migrations;

namespace Sc.Credits.Applications.AppServices.infrastructure.migrations
{
    public partial class AddColumnsValueCanCelAndCancellationType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CancellationType",
                table: "RequestCancelCredits",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<decimal>(
                name: "ValueCancel",
                table: "RequestCancelCredits",
                type: "decimal(26, 6)",
                nullable: true,
                defaultValue: null);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CancellationType",
                table: "RequestCancelCredits");

            migrationBuilder.DropColumn(
                name: "ValueCancel",
                table: "RequestCancelCredits");
        }
    }
}