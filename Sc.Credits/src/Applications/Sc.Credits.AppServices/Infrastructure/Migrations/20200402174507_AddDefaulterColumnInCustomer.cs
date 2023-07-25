using Microsoft.EntityFrameworkCore.Migrations;

namespace Sc.Credits.Applications.AppServices.Infrastructure.Migrations
{
    public partial class AddDefaulterColumnInCustomer : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Defaulter",
                table: "Customers",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Defaulter",
                table: "Customers");
        }
    }
}
