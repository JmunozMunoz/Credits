using Microsoft.EntityFrameworkCore.Migrations;

namespace Sc.Credits.AppServices.Infrastructure.Migrations
{
    public partial class SendTokenMailAndSms : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "SendTokenMail",
                table: "Customers",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "SendTokenSms",
                table: "Customers",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SendTokenMail",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "SendTokenSms",
                table: "Customers");
        }
    }
}