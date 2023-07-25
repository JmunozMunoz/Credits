using Microsoft.EntityFrameworkCore.Migrations;

namespace Sc.Credits.Applications.AppServices.Infrastructure.Migrations
{
    public partial class AddUserName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "UserName",
                table: "RequestCancelPayments",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.AddColumn<string>(
                name: "ProcessUserId",
                table: "RequestCancelPayments",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProcessUserName",
                table: "RequestCancelPayments",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "RequestCancelPayments",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "UserName",
                table: "RequestCancelCredits",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.AddColumn<string>(
                name: "ProcessUserId",
                table: "RequestCancelCredits",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProcessUserName",
                table: "RequestCancelCredits",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "RequestCancelCredits",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Credits",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 50);

            migrationBuilder.AddColumn<string>(
                name: "UserName",
                table: "Credits",
                maxLength: 100,
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProcessUserId",
                table: "RequestCancelPayments");

            migrationBuilder.DropColumn(
                name: "ProcessUserName",
                table: "RequestCancelPayments");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "RequestCancelPayments");

            migrationBuilder.DropColumn(
                name: "ProcessUserId",
                table: "RequestCancelCredits");

            migrationBuilder.DropColumn(
                name: "ProcessUserName",
                table: "RequestCancelCredits");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "RequestCancelCredits");

            migrationBuilder.DropColumn(
                name: "UserName",
                table: "Credits");

            migrationBuilder.AlterColumn<string>(
                name: "UserName",
                table: "RequestCancelPayments",
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "UserName",
                table: "RequestCancelCredits",
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Credits",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 100);
        }
    }
}
