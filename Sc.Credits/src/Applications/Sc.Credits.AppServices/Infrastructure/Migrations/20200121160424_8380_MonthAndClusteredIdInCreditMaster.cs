using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Sc.Credits.Applications.AppServices.Infrastructure.Migrations
{
    public partial class _8380_MonthAndClusteredIdInCreditMaster : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Credits_CreditsMaster_CreditMasterId",
                table: "Credits");

            migrationBuilder.DropForeignKey(
                name: "FK_RequestCancelCredits_CreditsMaster_CreditMasterId",
                table: "RequestCancelCredits");

            migrationBuilder.DropForeignKey(
                name: "FK_RequestCancelPayments_CreditsMaster_CreditMasterId",
                table: "RequestCancelPayments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CreditsMaster",
                table: "CreditsMaster");

            migrationBuilder.AddColumn<long>(
                name: "ClusteredId",
                table: "CreditsMaster",
                nullable: false,
                defaultValue: 0L)
                .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddColumn<int>(
                name: "Month",
                table: "CreditsMaster",
                nullable: false,
                computedColumnSql: "MONTH(CreditDate)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CreditsMaster",
                table: "CreditsMaster",
                column: "Id")
                .Annotation("SqlServer:Clustered", false);

            migrationBuilder.CreateIndex(
                name: "IX_CreditsMaster_ClusteredId",
                table: "CreditsMaster",
                column: "ClusteredId",
                unique: true)
                .Annotation("SqlServer:Clustered", true);

            migrationBuilder.AddForeignKey(
                name: "FK_Credits_CreditsMaster_CreditMasterId",
                table: "Credits",
                column: "CreditMasterId",
                principalTable: "CreditsMaster",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RequestCancelCredits_CreditsMaster_CreditMasterId",
                table: "RequestCancelCredits",
                column: "CreditMasterId",
                principalTable: "CreditsMaster",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RequestCancelPayments_CreditsMaster_CreditMasterId",
                table: "RequestCancelPayments",
                column: "CreditMasterId",
                principalTable: "CreditsMaster",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Credits_CreditsMaster_CreditMasterId",
                table: "Credits");

            migrationBuilder.DropForeignKey(
                name: "FK_RequestCancelCredits_CreditsMaster_CreditMasterId",
                table: "RequestCancelCredits");

            migrationBuilder.DropForeignKey(
                name: "FK_RequestCancelPayments_CreditsMaster_CreditMasterId",
                table: "RequestCancelPayments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CreditsMaster",
                table: "CreditsMaster");

            migrationBuilder.DropIndex(
                name: "IX_CreditsMaster_ClusteredId",
                table: "CreditsMaster");

            migrationBuilder.DropColumn(
                name: "ClusteredId",
                table: "CreditsMaster");

            migrationBuilder.DropColumn(
                name: "Month",
                table: "CreditsMaster");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CreditsMaster",
                table: "CreditsMaster",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Credits_CreditsMaster_CreditMasterId",
                table: "Credits",
                column: "CreditMasterId",
                principalTable: "CreditsMaster",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
               name: "FK_RequestCancelCredits_CreditsMaster_CreditMasterId",
               table: "RequestCancelCredits",
               column: "CreditMasterId",
               principalTable: "CreditsMaster",
               principalColumn: "Id",
               onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RequestCancelPayments_CreditsMaster_CreditMasterId",
                table: "RequestCancelPayments",
                column: "CreditMasterId",
                principalTable: "CreditsMaster",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}