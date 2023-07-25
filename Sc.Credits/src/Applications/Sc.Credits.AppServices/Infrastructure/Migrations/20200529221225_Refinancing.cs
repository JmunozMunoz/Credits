using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Sc.Credits.Applications.AppServices.Infrastructure.Migrations
{
    public partial class Refinancing : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RefinancingApplications",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    ArrearsDaysFrom = table.Column<int>(nullable: false),
                    ArrearsDaysTo = table.Column<int>(nullable: false),
                    CreationDateFrom = table.Column<DateTime>(type: "date", nullable: false),
                    CreationDateTo = table.Column<DateTime>(type: "date", nullable: false),
                    Name = table.Column<string>(maxLength: 200, nullable: false),
                    ValueFrom = table.Column<decimal>(type: "decimal(26, 6)", nullable: false),
                    ValueTo = table.Column<decimal>(type: "decimal(26, 6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefinancingApplications", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RefinancingLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Date = table.Column<DateTime>(nullable: false),
                    ApplicationId = table.Column<Guid>(nullable: false),
                    CreditMasterId = table.Column<Guid>(nullable: false),
                    Value = table.Column<decimal>(type: "decimal(26, 6)", nullable: false),
                    StoreId = table.Column<string>(nullable: false),
                    CustomerId = table.Column<Guid>(nullable: false),
                    ReferenceText = table.Column<string>(nullable: false),
                    ReferenceCode = table.Column<string>(nullable: false),
                    UserId = table.Column<string>(maxLength: 100, nullable: false),
                    UserName = table.Column<string>(maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefinancingLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RefinancingLogs_RefinancingApplications_ApplicationId",
                        column: x => x.ApplicationId,
                        principalTable: "RefinancingApplications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RefinancingLogs_CreditsMaster_CreditMasterId",
                        column: x => x.CreditMasterId,
                        principalTable: "CreditsMaster",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RefinancingLogs_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RefinancingLogs_Stores_StoreId",
                        column: x => x.StoreId,
                        principalTable: "Stores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RefinancingLogDetails",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    RefinancingLogId = table.Column<Guid>(nullable: false),
                    CreditMasterId = table.Column<Guid>(nullable: false),
                    CreditId = table.Column<Guid>(nullable: false),
                    Value = table.Column<decimal>(type: "decimal(26, 6)", nullable: false),
                    RefinancingLogId1 = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefinancingLogDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RefinancingLogDetails_Credits_CreditId",
                        column: x => x.CreditId,
                        principalTable: "Credits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RefinancingLogDetails_CreditsMaster_CreditMasterId",
                        column: x => x.CreditMasterId,
                        principalTable: "CreditsMaster",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RefinancingLogDetails_RefinancingLogs_RefinancingLogId",
                        column: x => x.RefinancingLogId,
                        principalTable: "RefinancingLogs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RefinancingLogDetails_RefinancingLogs_RefinancingLogId1",
                        column: x => x.RefinancingLogId1,
                        principalTable: "RefinancingLogs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RefinancingLogDetails_CreditId",
                table: "RefinancingLogDetails",
                column: "CreditId");

            migrationBuilder.CreateIndex(
                name: "IX_RefinancingLogDetails_CreditMasterId",
                table: "RefinancingLogDetails",
                column: "CreditMasterId");

            migrationBuilder.CreateIndex(
                name: "IX_RefinancingLogDetails_RefinancingLogId",
                table: "RefinancingLogDetails",
                column: "RefinancingLogId");

            migrationBuilder.CreateIndex(
                name: "IX_RefinancingLogDetails_RefinancingLogId1",
                table: "RefinancingLogDetails",
                column: "RefinancingLogId1");

            migrationBuilder.CreateIndex(
                name: "IX_RefinancingLogs_ApplicationId",
                table: "RefinancingLogs",
                column: "ApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_RefinancingLogs_CreditMasterId",
                table: "RefinancingLogs",
                column: "CreditMasterId");

            migrationBuilder.CreateIndex(
                name: "IX_RefinancingLogs_CustomerId",
                table: "RefinancingLogs",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_RefinancingLogs_StoreId",
                table: "RefinancingLogs",
                column: "StoreId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RefinancingLogDetails");

            migrationBuilder.DropTable(
                name: "RefinancingLogs");

            migrationBuilder.DropTable(
                name: "RefinancingApplications");
        }
    }
}
