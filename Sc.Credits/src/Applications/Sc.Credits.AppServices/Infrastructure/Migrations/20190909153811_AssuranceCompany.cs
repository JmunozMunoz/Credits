using Microsoft.EntityFrameworkCore.Migrations;

namespace Sc.Credits.Applications.AppServices.Infrastructure.Migrations
{
    public partial class AssuranceCompany : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateSequence(
                name: "AssuranceCompaniesSeq",
                incrementBy: 10);

            migrationBuilder.AddColumn<long>(
                name: "AssuranceCompanyId",
                table: "Stores",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AssuranceCompanies",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false),
                    Name = table.Column<string>(maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssuranceCompanies", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Stores_AssuranceCompanyId",
                table: "Stores",
                column: "AssuranceCompanyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Stores_AssuranceCompanies_AssuranceCompanyId",
                table: "Stores",
                column: "AssuranceCompanyId",
                principalTable: "AssuranceCompanies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Stores_AssuranceCompanies_AssuranceCompanyId",
                table: "Stores");

            migrationBuilder.DropTable(
                name: "AssuranceCompanies");

            migrationBuilder.DropIndex(
                name: "IX_Stores_AssuranceCompanyId",
                table: "Stores");

            migrationBuilder.DropSequence(
                name: "AssuranceCompaniesSeq");

            migrationBuilder.DropColumn(
                name: "AssuranceCompanyId",
                table: "Stores");
        }
    }
}
