using Microsoft.EntityFrameworkCore.Migrations;

namespace Sc.Credits.Applications.AppServices.infrastructure.migrations
{
    public partial class AddColumn_StoreProfileCode : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "StoreProfileCode",
                table: "Stores",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StoreProfileCode",
                table: "Stores");
        }
    }
}
