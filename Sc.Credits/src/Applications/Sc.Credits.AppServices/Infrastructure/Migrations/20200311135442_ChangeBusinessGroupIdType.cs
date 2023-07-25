using Microsoft.EntityFrameworkCore.Migrations;

namespace Sc.Credits.Applications.AppServices.Infrastructure.Migrations
{
    public partial class ChangeBusinessGroupIdType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropSequence(
                name: "BusinessGroupSeq");

            migrationBuilder.DropIndex(name: "IX_Stores_BusinessGroupId",
                table: "Stores");

            migrationBuilder.DropForeignKey(name: "FK_Stores_BusinessGroup_BusinessGroupId",
                table: "Stores");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BusinessGroup",
                table: "BusinessGroup");

            migrationBuilder.AlterColumn<string>(
                name: "BusinessGroupId",
                table: "Stores",
                nullable: true,
                oldClrType: typeof(long),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "BusinessGroup",
                nullable: false,
                oldClrType: typeof(long));

            migrationBuilder.AddPrimaryKey(
               name: "PK_BusinessGroup",
               table: "BusinessGroup",
               column: "Id");

            migrationBuilder.AddForeignKey(name: "FK_Stores_BusinessGroup_BusinessGroupId",
                table: "Stores",
                column: "BusinessGroupId",
                principalTable: "BusinessGroup",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.CreateIndex(name: "IX_Stores_BusinessGroupId",
                table: "Stores",
                column: "BusinessGroupId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateSequence(
                name: "BusinessGroupSeq",
                incrementBy: 10);

            migrationBuilder.DropIndex(name: "IX_Stores_BusinessGroupId",
                table: "Stores");

            migrationBuilder.DropForeignKey(name: "FK_Stores_BusinessGroup_BusinessGroupId",
               table: "Stores");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BusinessGroup",
                table: "BusinessGroup");

            migrationBuilder.AlterColumn<long>(
                name: "BusinessGroupId",
                table: "Stores",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "BusinessGroup",
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.AddPrimaryKey(
               name: "PK_BusinessGroup",
               table: "BusinessGroup",
               column: "Id");

            migrationBuilder.AddForeignKey(name: "FK_Stores_BusinessGroup_BusinessGroupId",
                table: "Stores",
                column: "BusinessGroupId",
                principalTable: "BusinessGroup",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.CreateIndex(name: "IX_Stores_BusinessGroupId",
                table: "Stores",
                column: "BusinessGroupId");
        }
    }
}