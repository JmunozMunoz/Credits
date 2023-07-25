using Microsoft.EntityFrameworkCore.Migrations;

namespace Sc.Credits.Applications.AppServices.Infrastructure.Migrations
{
    public partial class AddBusinessGroup : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateSequence(
                name: "BusinessGroupSeq",
                incrementBy: 10);

            migrationBuilder.AlterColumn<long>(
                name: "BusinessGroupId",
                table: "Stores",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 50);

            migrationBuilder.CreateTable(
                name: "BusinessGroup",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false),
                    Name = table.Column<string>(maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BusinessGroup", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Stores_BusinessGroup_BusinessGroupId",
                table: "Stores",
                column: "BusinessGroupId",
                principalTable: "BusinessGroup",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Stores_BusinessGroup_BusinessGroupId",
                table: "Stores");

            migrationBuilder.DropTable(
                name: "BusinessGroup");

            migrationBuilder.DropSequence(
                name: "BusinessGroupSeq");

            migrationBuilder.AlterColumn<string>(
                name: "BusinessGroupId",
                table: "Stores",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(long),
                oldNullable: true);
        }
    }
}
