using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Sc.Credits.Applications.AppServices.Infrastructure.Migrations
{
    public partial class RefinancingLogBugFix : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RefinancingLogDetails_RefinancingLogs_RefinancingLogId1",
                table: "RefinancingLogDetails");

            migrationBuilder.DropIndex(
                name: "IX_RefinancingLogDetails_RefinancingLogId1",
                table: "RefinancingLogDetails");

            migrationBuilder.DropColumn(
                name: "RefinancingLogId1",
                table: "RefinancingLogDetails");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "RefinancingLogId1",
                table: "RefinancingLogDetails",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_RefinancingLogDetails_RefinancingLogId1",
                table: "RefinancingLogDetails",
                column: "RefinancingLogId1");

            migrationBuilder.AddForeignKey(
                name: "FK_RefinancingLogDetails_RefinancingLogs_RefinancingLogId1",
                table: "RefinancingLogDetails",
                column: "RefinancingLogId1",
                principalTable: "RefinancingLogs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
