using Microsoft.EntityFrameworkCore.Migrations;

namespace Sc.Credits.Applications.AppServices.Infrastructure.Migrations
{
    public partial class UniqueCreditIndex : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Hour",
                table: "CreditsMaster",
                nullable: false,
                computedColumnSql: "DATEPART(HOUR, CreditTime) PERSISTED");

            migrationBuilder.AddColumn<int>(
                name: "Minute",
                table: "CreditsMaster",
                nullable: false,
                computedColumnSql: "DATEPART(MINUTE, CreditTime) PERSISTED");

            migrationBuilder.Sql(@"CREATE UNIQUE INDEX IX_CreditsMaster_CustomerId_StoreId_CreditDate_Month_Hour_Minute
                                ON CreditsMaster (CustomerId, StoreId, CreditDate, Month, Hour, Minute)
                                ON PS_MonthCreditsPartition(Month)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CreditsMaster_CustomerId_StoreId_CreditDate_Month_Hour_Minute",
                table: "CreditsMaster");

            migrationBuilder.DropColumn(
                name: "Hour",
                table: "CreditsMaster");

            migrationBuilder.DropColumn(
                name: "Minute",
                table: "CreditsMaster");
        }
    }
}