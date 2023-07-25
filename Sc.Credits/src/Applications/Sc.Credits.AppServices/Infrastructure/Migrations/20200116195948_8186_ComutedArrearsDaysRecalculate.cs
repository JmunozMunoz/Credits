using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Sc.Credits.Applications.AppServices.Infrastructure.Migrations
{
    public partial class _8186_ComutedArrearsDaysRecalculate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "CreditMasterId",
                table: "Credits",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ComputedArrearsDays",
                table: "Credits",
                nullable: false,
                computedColumnSql: "(CASE WHEN DATEDIFF(DAY, [LastPaymentDate], GETDATE()) <= 0 THEN 0 ELSE DATEDIFF(DAY, [LastPaymentDate], GETDATE()) END)",
                oldClrType: typeof(int),
                oldComputedColumnSql: "(CASE WHEN DATEDIFF(DAY,(CASE WHEN [DueDate] > [LastPaymentDate] THEN [DueDate] ELSE [LastPaymentDate] END),GETDATE()) <= 0 THEN 0 ELSE DATEDIFF(DAY,(CASE WHEN [DueDate] > [LastPaymentDate] THEN [DueDate] ELSE [LastPaymentDate] END),GETDATE()) END)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "CreditMasterId",
                table: "Credits",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<int>(
                name: "ComputedArrearsDays",
                table: "Credits",
                nullable: false,
                computedColumnSql: "(CASE WHEN DATEDIFF(DAY,(CASE WHEN [DueDate] > [LastPaymentDate] THEN [DueDate] ELSE [LastPaymentDate] END),GETDATE()) <= 0 THEN 0 ELSE DATEDIFF(DAY,(CASE WHEN [DueDate] > [LastPaymentDate] THEN [DueDate] ELSE [LastPaymentDate] END),GETDATE()) END)",
                oldClrType: typeof(int),
                oldComputedColumnSql: "(CASE WHEN DATEDIFF(DAY, [LastPaymentDate], GETDATE()) <= 0 THEN 0 ELSE DATEDIFF(DAY, [LastPaymentDate], GETDATE()) END)");
        }
    }
}
