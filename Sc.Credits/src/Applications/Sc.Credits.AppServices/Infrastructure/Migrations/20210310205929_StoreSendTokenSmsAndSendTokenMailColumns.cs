using Microsoft.EntityFrameworkCore.Migrations;

namespace Sc.Credits.Applications.AppServices.infrastructure.migrations
{
    public partial class StoreSendTokenSmsAndSendTokenMailColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "SendTokenMail",
                table: "Stores",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "SendTokenSms",
                table: "Stores",
                nullable: false,
                defaultValue: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SendTokenMail",
                table: "Stores");

            migrationBuilder.DropColumn(
                name: "SendTokenSms",
                table: "Stores");
        }
    }
}