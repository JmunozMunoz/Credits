using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Sc.Credits.Applications.AppServices.Infrastructure.Migrations
{
    public partial class CreditsAndCancelsFixes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PaymentDetails");

            migrationBuilder.DropTable(
                name: "PaymentStatuses");

            migrationBuilder.DropSequence(
                name: "PaymentStatusesSeq");

            migrationBuilder.CreateSequence(
                name: "RequestStatusSeq",
                incrementBy: 10);

            migrationBuilder.AddColumn<int>(
                name: "PaymentTypeId",
                table: "Stores",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "RequestStatus",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequestStatus", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RequestCancelCredits",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreditMasterId = table.Column<Guid>(nullable: true),
                    Date = table.Column<DateTime>(nullable: false),
                    ProcessDate = table.Column<DateTime>(nullable: true),
                    ProcessTime = table.Column<TimeSpan>(nullable: true),
                    Reason = table.Column<string>(nullable: false),
                    RequestStatusId = table.Column<int>(nullable: true),
                    StoreId = table.Column<string>(nullable: true),
                    Time = table.Column<TimeSpan>(nullable: false),
                    UserName = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequestCancelCredits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RequestCancelCredits_CreditsMaster_CreditMasterId",
                        column: x => x.CreditMasterId,
                        principalTable: "CreditsMaster",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RequestCancelCredits_RequestStatus_RequestStatusId",
                        column: x => x.RequestStatusId,
                        principalTable: "RequestStatus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RequestCancelCredits_Stores_StoreId",
                        column: x => x.StoreId,
                        principalTable: "Stores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RequestCancelPayments",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreditCancelId = table.Column<Guid>(nullable: true),
                    CreditId = table.Column<Guid>(nullable: true),
                    CreditMasterId = table.Column<Guid>(nullable: true),
                    Date = table.Column<DateTime>(nullable: false),
                    ProcessDate = table.Column<DateTime>(nullable: true),
                    ProcessTime = table.Column<TimeSpan>(nullable: true),
                    Reason = table.Column<string>(nullable: false),
                    RequestStatusId = table.Column<int>(nullable: true),
                    StoreId = table.Column<string>(nullable: true),
                    Time = table.Column<TimeSpan>(nullable: false),
                    UserName = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequestCancelPayments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RequestCancelPayments_Credits_CreditCancelId",
                        column: x => x.CreditCancelId,
                        principalTable: "Credits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RequestCancelPayments_Credits_CreditId",
                        column: x => x.CreditId,
                        principalTable: "Credits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RequestCancelPayments_CreditsMaster_CreditMasterId",
                        column: x => x.CreditMasterId,
                        principalTable: "CreditsMaster",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RequestCancelPayments_RequestStatus_RequestStatusId",
                        column: x => x.RequestStatusId,
                        principalTable: "RequestStatus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RequestCancelPayments_Stores_StoreId",
                        column: x => x.StoreId,
                        principalTable: "Stores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Stores_PaymentTypeId",
                table: "Stores",
                column: "PaymentTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_RequestCancelCredits_CreditMasterId",
                table: "RequestCancelCredits",
                column: "CreditMasterId");

            migrationBuilder.CreateIndex(
                name: "IX_RequestCancelCredits_RequestStatusId",
                table: "RequestCancelCredits",
                column: "RequestStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_RequestCancelCredits_StoreId",
                table: "RequestCancelCredits",
                column: "StoreId");

            migrationBuilder.CreateIndex(
                name: "IX_RequestCancelPayments_CreditCancelId",
                table: "RequestCancelPayments",
                column: "CreditCancelId");

            migrationBuilder.CreateIndex(
                name: "IX_RequestCancelPayments_CreditId",
                table: "RequestCancelPayments",
                column: "CreditId");

            migrationBuilder.CreateIndex(
                name: "IX_RequestCancelPayments_CreditMasterId",
                table: "RequestCancelPayments",
                column: "CreditMasterId");

            migrationBuilder.CreateIndex(
                name: "IX_RequestCancelPayments_RequestStatusId",
                table: "RequestCancelPayments",
                column: "RequestStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_RequestCancelPayments_StoreId",
                table: "RequestCancelPayments",
                column: "StoreId");

            migrationBuilder.AddForeignKey(
                name: "FK_Stores_PaymentTypes_PaymentTypeId",
                table: "Stores",
                column: "PaymentTypeId",
                principalTable: "PaymentTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Stores_PaymentTypes_PaymentTypeId",
                table: "Stores");

            migrationBuilder.DropTable(
                name: "RequestCancelCredits");

            migrationBuilder.DropTable(
                name: "RequestCancelPayments");

            migrationBuilder.DropTable(
                name: "RequestStatus");

            migrationBuilder.DropIndex(
                name: "IX_Stores_PaymentTypeId",
                table: "Stores");

            migrationBuilder.DropSequence(
                name: "RequestStatusSeq");

            migrationBuilder.DropColumn(
                name: "PaymentTypeId",
                table: "Stores");

            migrationBuilder.CreateSequence(
                name: "PaymentStatusesSeq",
                incrementBy: 10);

            migrationBuilder.CreateTable(
                name: "PaymentStatuses",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentStatuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PaymentDetails",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CancelDate = table.Column<DateTime>(nullable: true),
                    CancelTime = table.Column<TimeSpan>(nullable: true),
                    CreditCancelId = table.Column<Guid>(nullable: true),
                    CreditId = table.Column<Guid>(nullable: false),
                    CreditMasterId = table.Column<Guid>(nullable: false),
                    CustomerId = table.Column<Guid>(nullable: true),
                    PaymentStatusId = table.Column<int>(nullable: true),
                    PaymentStatusName = table.Column<string>(nullable: false),
                    Reason = table.Column<string>(nullable: true),
                    StoreId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PaymentDetails_Credits_CreditId",
                        column: x => x.CreditId,
                        principalTable: "Credits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PaymentDetails_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PaymentDetails_PaymentStatuses_PaymentStatusId",
                        column: x => x.PaymentStatusId,
                        principalTable: "PaymentStatuses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PaymentDetails_Stores_StoreId",
                        column: x => x.StoreId,
                        principalTable: "Stores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PaymentDetails_CreditCancelId",
                table: "PaymentDetails",
                column: "CreditCancelId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentDetails_CreditId",
                table: "PaymentDetails",
                column: "CreditId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PaymentDetails_CreditMasterId",
                table: "PaymentDetails",
                column: "CreditMasterId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentDetails_CustomerId",
                table: "PaymentDetails",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentDetails_PaymentStatusId",
                table: "PaymentDetails",
                column: "PaymentStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentDetails_StoreId",
                table: "PaymentDetails",
                column: "StoreId");
        }
    }
}
