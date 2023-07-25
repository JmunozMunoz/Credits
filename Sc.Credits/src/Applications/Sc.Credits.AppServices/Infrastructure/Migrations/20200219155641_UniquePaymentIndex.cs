using Microsoft.EntityFrameworkCore.Migrations;

namespace Sc.Credits.Applications.AppServices.Infrastructure.Migrations
{
    public partial class UniquePaymentIndex : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TransactionHour",
                table: "Credits",
                nullable: false,
                computedColumnSql: "DATEPART(HOUR, TransactionTime) PERSISTED");

            migrationBuilder.AddColumn<int>(
                name: "TransactionMinute",
                table: "Credits",
                nullable: false,
                computedColumnSql: "DATEPART(MINUTE, TransactionTime) PERSISTED");

            migrationBuilder.CreateIndex(
                name: "IX_Credits_CreditMasterId_TotalValuePaid_TransactionDate_TransactionHour_TransactionMinute",
                table: "Credits",
                columns: new[] { "CreditMasterId", "TotalValuePaid", "TransactionDate", "TransactionHour", "TransactionMinute" },
                unique: true,
                filter: "[TransactionTypeId] = 2 AND [TransactionDate] > '2020-02-18'");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Credits_CreditMasterId_TotalValuePaid_TransactionDate_TransactionHour_TransactionMinute",
                table: "Credits");

            migrationBuilder.DropColumn(
                name: "TransactionHour",
                table: "Credits");

            migrationBuilder.DropColumn(
                name: "TransactionMinute",
                table: "Credits");
        }
    }
}