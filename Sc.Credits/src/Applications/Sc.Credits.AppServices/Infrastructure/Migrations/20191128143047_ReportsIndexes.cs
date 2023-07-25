using Microsoft.EntityFrameworkCore.Migrations;

namespace Sc.Credits.Applications.AppServices.Infrastructure.Migrations
{
    public partial class ReportsIndexes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_RequestCancelPayments_ProcessDate",
                table: "RequestCancelPayments",
                column: "ProcessDate");

            migrationBuilder.CreateIndex(
                name: "IX_RequestCancelCredits_ProcessDate",
                table: "RequestCancelCredits",
                column: "ProcessDate");

            migrationBuilder.CreateIndex(
                name: "IX_CreditsMaster_CreditDate",
                table: "CreditsMaster",
                column: "CreditDate");

            migrationBuilder.CreateIndex(
                name: "IX_Credits_TransactionDate",
                table: "Credits",
                column: "TransactionDate");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_RequestCancelPayments_ProcessDate",
                table: "RequestCancelPayments");

            migrationBuilder.DropIndex(
                name: "IX_RequestCancelCredits_ProcessDate",
                table: "RequestCancelCredits");

            migrationBuilder.DropIndex(
                name: "IX_CreditsMaster_CreditDate",
                table: "CreditsMaster");

            migrationBuilder.DropIndex(
                name: "IX_Credits_TransactionDate",
                table: "Credits");
        }
    }
}
