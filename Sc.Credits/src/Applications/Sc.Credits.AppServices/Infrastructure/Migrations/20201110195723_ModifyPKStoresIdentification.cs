using Microsoft.EntityFrameworkCore.Migrations;

namespace Sc.Credits.Applications.AppServices.Infrastructure.Migrations
{
    public partial class ModifyPKStoresIdentification : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_StoreIdentifications",
                table: "StoreIdentifications");

            migrationBuilder.DropSequence(
                name: "StoreIdentificationsSeq");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "StoreIdentifications");

            migrationBuilder.AddPrimaryKey(
                name: "PK_StoreIdentifications",
                table: "StoreIdentifications",
                columns: new[] { "StoreId", "ScCode" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_StoreIdentifications",
                table: "StoreIdentifications");

            migrationBuilder.CreateSequence(
                name: "StoreIdentificationsSeq",
                incrementBy: 10);

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "StoreIdentifications",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_StoreIdentifications",
                table: "StoreIdentifications",
                column: "Id");
        }
    }
}
