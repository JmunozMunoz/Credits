using Microsoft.EntityFrameworkCore.Migrations;

namespace Sc.Credits.Applications.AppServices.Infrastructure.Migrations
{
    public partial class DropTokenIndex : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CreditsMaster_Token",
                table: "CreditsMaster");

            migrationBuilder.AlterColumn<string>(
                name: "Token",
                table: "CreditsMaster",
                nullable: false,
                oldClrType: typeof(string));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Token",
                table: "CreditsMaster",
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.CreateIndex(
                name: "IX_CreditsMaster_Token",
                table: "CreditsMaster",
                column: "Token",
                unique: true);
        }
    }
}
