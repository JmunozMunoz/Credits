using Microsoft.EntityFrameworkCore.Migrations;

namespace Sc.Credits.Applications.AppServices.infrastructure.migrations
{
    public partial class AgentAnalysesWithSourceRelated : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Source",
                table: "CreditRequestAgentAnalyses");

            migrationBuilder.AddColumn<int>(
                name: "SourceId",
                table: "CreditRequestAgentAnalyses",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.CreateIndex(
                name: "IX_CreditRequestAgentAnalyses_SourceId",
                table: "CreditRequestAgentAnalyses",
                column: "SourceId");

            migrationBuilder.AddForeignKey(
                name: "FK_CreditRequestAgentAnalyses_Sources_SourceId",
                table: "CreditRequestAgentAnalyses",
                column: "SourceId",
                principalTable: "Sources",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CreditRequestAgentAnalyses_Sources_SourceId",
                table: "CreditRequestAgentAnalyses");

            migrationBuilder.DropIndex(
                name: "IX_CreditRequestAgentAnalyses_SourceId",
                table: "CreditRequestAgentAnalyses");

            migrationBuilder.DropColumn(
                name: "SourceId",
                table: "CreditRequestAgentAnalyses");

            migrationBuilder.AddColumn<string>(
                name: "Source",
                table: "CreditRequestAgentAnalyses",
                nullable: false,
                defaultValue: "");
        }
    }
}