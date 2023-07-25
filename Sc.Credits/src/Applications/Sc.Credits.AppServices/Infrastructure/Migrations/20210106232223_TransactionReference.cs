using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Sc.Credits.Applications.AppServices.infrastructure.migrations
{
    public partial class TransactionReference : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TransactionReferences",
                columns: table => new
                {
                    TransactionId = table.Column<string>(maxLength: 50, nullable: false),
                    CreditId = table.Column<Guid>(maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransactionReferences", x => new { x.CreditId, x.TransactionId });
                    table.ForeignKey(
                        name: "FK_TransactionReferences_Credits_CreditId",
                        column: x => x.CreditId,
                        principalTable: "Credits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TransactionReferences_CreditId",
                table: "TransactionReferences",
                column: "CreditId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TransactionReferences");
        }
    }
}
