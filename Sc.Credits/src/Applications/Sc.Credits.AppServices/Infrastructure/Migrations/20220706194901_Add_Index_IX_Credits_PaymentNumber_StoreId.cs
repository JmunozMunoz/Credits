using Microsoft.EntityFrameworkCore.Migrations;

namespace Sc.Credits.Applications.AppServices.infrastructure.migrations
{
    public partial class Add_Index_IX_Credits_PaymentNumber_StoreId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Credits_PaymentNumber_StoreId",
                table: "Credits",
                columns: new[] { "PaymentNumber", "StoreId" },
                unique: true,
                filter: "[CreateDate] >= '2022-07-06' AND [TransactionTypeId]=2");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Credits_PaymentNumber_StoreId",
                table: "Credits");
        }
    }
}
