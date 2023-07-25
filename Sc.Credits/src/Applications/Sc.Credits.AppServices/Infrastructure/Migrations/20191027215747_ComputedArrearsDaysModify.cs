using Microsoft.EntityFrameworkCore.Migrations;

namespace Sc.Credits.Applications.AppServices.Infrastructure.Migrations
{
    public partial class ComputedArrearsDaysModify : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "ComputedArrearsDays",
                table: "Credits",
                nullable: false,
                computedColumnSql: "(CASE WHEN DATEDIFF(DAY,(CASE WHEN [DueDate] > [LastPaymentDate] THEN [DueDate] ELSE [LastPaymentDate] END),GETDATE()) <= 0 THEN 0 ELSE DATEDIFF(DAY,(CASE WHEN [DueDate] > [LastPaymentDate] THEN [DueDate] ELSE [LastPaymentDate] END),GETDATE()) END)",
                oldClrType: typeof(int),
                oldComputedColumnSql: "(CASE WHEN DATEDIFF(DAY,[DueDate],GETDATE()) <= 0 THEN 0 ELSE DATEDIFF(DAY,[DueDate],GETDATE()) END)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "ComputedArrearsDays",
                table: "Credits",
                nullable: false,
                computedColumnSql: "(CASE WHEN DATEDIFF(DAY,[DueDate],GETDATE()) <= 0 THEN 0 ELSE DATEDIFF(DAY,[DueDate],GETDATE()) END)",
                oldClrType: typeof(int),
                oldComputedColumnSql: "(CASE WHEN DATEDIFF(DAY,(CASE WHEN [DueDate] > [LastPaymentDate] THEN [DueDate] ELSE [LastPaymentDate] END),GETDATE()) <= 0 THEN 0 ELSE DATEDIFF(DAY,(CASE WHEN [DueDate] > [LastPaymentDate] THEN [DueDate] ELSE [LastPaymentDate] END),GETDATE()) END)");
        }
    }
}
