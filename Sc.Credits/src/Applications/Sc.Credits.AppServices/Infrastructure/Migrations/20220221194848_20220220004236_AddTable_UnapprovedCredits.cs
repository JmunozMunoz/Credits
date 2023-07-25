using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Sc.Credits.Applications.AppServices.infrastructure.migrations
{
    public partial class _20220220004236_AddTable_UnapprovedCredits : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UnapprovedCredits",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CustomerId = table.Column<Guid>(nullable: false),
                    StoreId = table.Column<string>(nullable: false),
                    TransactionDate = table.Column<DateTime>(type: "date", nullable: false),
                    TransactionTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    CreditValue = table.Column<decimal>(type: "decimal(26, 6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnapprovedCredits", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UnapprovedCredits_TransactionDate",
                table: "UnapprovedCredits",
                column: "TransactionDate");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UnapprovedCredits");
        }
    }
}
