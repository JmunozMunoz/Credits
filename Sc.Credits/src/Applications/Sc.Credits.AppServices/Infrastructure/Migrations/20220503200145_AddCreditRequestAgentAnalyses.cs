using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Sc.Credits.Applications.AppServices.infrastructure.migrations
{
    public partial class AddCreditRequestAgentAnalyses : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AgentAnalysisResults",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 200, nullable: false),
                    Description = table.Column<string>(maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AgentAnalysisResults", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CreditRequestAgentAnalyses",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CustomerId = table.Column<Guid>(nullable: false),
                    CustomerIdDocument = table.Column<string>(maxLength: 50, nullable: false),
                    CreditValue = table.Column<decimal>(type: "decimal(26, 6)", nullable: false),
                    Observations = table.Column<string>(nullable: false),
                    StoreId = table.Column<string>(nullable: false),
                    TransactionDate = table.Column<DateTime>(type: "date", nullable: false),
                    TransactionTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    AgentAnalysisResultId = table.Column<int>(nullable: false),
                    Source = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CreditRequestAgentAnalyses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CreditRequestAgentAnalyses_AgentAnalysisResults_AgentAnalysisResultId",
                        column: x => x.AgentAnalysisResultId,
                        principalTable: "AgentAnalysisResults",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CreditRequestAgentAnalyses_AgentAnalysisResultId",
                table: "CreditRequestAgentAnalyses",
                column: "AgentAnalysisResultId");

            migrationBuilder.CreateIndex(
                name: "IX_CreditRequestAgentAnalyses_TransactionDate",
                table: "CreditRequestAgentAnalyses",
                column: "TransactionDate");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CreditRequestAgentAnalyses");

            migrationBuilder.DropTable(
                name: "AgentAnalysisResults");
        }
    }
}