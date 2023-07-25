using Microsoft.EntityFrameworkCore.Migrations;

namespace Sc.Credits.Applications.AppServices.infrastructure.migrations
{
    public partial class StoreCategoryIdColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "StoreCategoryId",
                table: "Stores",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.CreateIndex(
                name: "IX_Stores_StoreCategoryId",
                table: "Stores",
                column: "StoreCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Stores_StoresCategories_StoreCategoryId",
                table: "Stores",
                column: "StoreCategoryId",
                principalTable: "StoresCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Stores_StoresCategories_StoreCategoryId",
                table: "Stores");

            migrationBuilder.DropIndex(
                name: "IX_Stores_StoreCategoryId",
                table: "Stores");

            migrationBuilder.DropColumn(
                name: "StoreCategoryId",
                table: "Stores");
        }
    }
}