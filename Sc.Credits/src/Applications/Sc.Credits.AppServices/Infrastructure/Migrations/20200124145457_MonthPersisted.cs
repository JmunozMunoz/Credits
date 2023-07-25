using Microsoft.EntityFrameworkCore.Migrations;

namespace Sc.Credits.Applications.AppServices.Infrastructure.Migrations
{
    public partial class MonthPersisted : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Month",
                table: "CreditsMaster",
                nullable: false,
                computedColumnSql: "MONTH(CreditDate) PERSISTED",
                oldClrType: typeof(int),
                oldComputedColumnSql: "MONTH(CreditDate)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Month",
                table: "CreditsMaster",
                nullable: false,
                computedColumnSql: "MONTH(CreditDate)",
                oldClrType: typeof(int),
                oldComputedColumnSql: "MONTH(CreditDate) PERSISTED");
        }
    }
}
