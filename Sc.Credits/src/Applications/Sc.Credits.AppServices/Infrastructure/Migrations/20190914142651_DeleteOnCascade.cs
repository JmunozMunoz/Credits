using Microsoft.EntityFrameworkCore.Migrations;

namespace Sc.Credits.Applications.AppServices.Infrastructure.Migrations
{
    public partial class DeleteOnCascade : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Credits_AuthMethods_AuthMethodId",
                table: "Credits");

            migrationBuilder.DropForeignKey(
                name: "FK_Credits_Customers_CustomerId",
                table: "Credits");

            migrationBuilder.DropForeignKey(
                name: "FK_Credits_Sources_SourceId",
                table: "Credits");

            migrationBuilder.DropForeignKey(
                name: "FK_Credits_Status_StatusId",
                table: "Credits");

            migrationBuilder.DropForeignKey(
                name: "FK_Credits_Stores_StoreId",
                table: "Credits");

            migrationBuilder.DropForeignKey(
                name: "FK_CreditsMaster_Customers_CustomerId",
                table: "CreditsMaster");

            migrationBuilder.DropForeignKey(
                name: "FK_CreditsMaster_Status_StatusId",
                table: "CreditsMaster");

            migrationBuilder.DropForeignKey(
                name: "FK_CreditsMaster_Stores_StoreId",
                table: "CreditsMaster");

            migrationBuilder.DropForeignKey(
                name: "FK_Customers_Profiles_ProfileId",
                table: "Customers");

            migrationBuilder.AddForeignKey(
                name: "FK_Credits_AuthMethods_AuthMethodId",
                table: "Credits",
                column: "AuthMethodId",
                principalTable: "AuthMethods",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Credits_Customers_CustomerId",
                table: "Credits",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Credits_Sources_SourceId",
                table: "Credits",
                column: "SourceId",
                principalTable: "Sources",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Credits_Status_StatusId",
                table: "Credits",
                column: "StatusId",
                principalTable: "Status",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Credits_Stores_StoreId",
                table: "Credits",
                column: "StoreId",
                principalTable: "Stores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CreditsMaster_Customers_CustomerId",
                table: "CreditsMaster",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CreditsMaster_Status_StatusId",
                table: "CreditsMaster",
                column: "StatusId",
                principalTable: "Status",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CreditsMaster_Stores_StoreId",
                table: "CreditsMaster",
                column: "StoreId",
                principalTable: "Stores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Customers_Profiles_ProfileId",
                table: "Customers",
                column: "ProfileId",
                principalTable: "Profiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Credits_AuthMethods_AuthMethodId",
                table: "Credits");

            migrationBuilder.DropForeignKey(
                name: "FK_Credits_Customers_CustomerId",
                table: "Credits");

            migrationBuilder.DropForeignKey(
                name: "FK_Credits_Sources_SourceId",
                table: "Credits");

            migrationBuilder.DropForeignKey(
                name: "FK_Credits_Status_StatusId",
                table: "Credits");

            migrationBuilder.DropForeignKey(
                name: "FK_Credits_Stores_StoreId",
                table: "Credits");

            migrationBuilder.DropForeignKey(
                name: "FK_CreditsMaster_Customers_CustomerId",
                table: "CreditsMaster");

            migrationBuilder.DropForeignKey(
                name: "FK_CreditsMaster_Status_StatusId",
                table: "CreditsMaster");

            migrationBuilder.DropForeignKey(
                name: "FK_CreditsMaster_Stores_StoreId",
                table: "CreditsMaster");

            migrationBuilder.DropForeignKey(
                name: "FK_Customers_Profiles_ProfileId",
                table: "Customers");

            migrationBuilder.AddForeignKey(
                name: "FK_Credits_AuthMethods_AuthMethodId",
                table: "Credits",
                column: "AuthMethodId",
                principalTable: "AuthMethods",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Credits_Customers_CustomerId",
                table: "Credits",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Credits_Sources_SourceId",
                table: "Credits",
                column: "SourceId",
                principalTable: "Sources",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Credits_Status_StatusId",
                table: "Credits",
                column: "StatusId",
                principalTable: "Status",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Credits_Stores_StoreId",
                table: "Credits",
                column: "StoreId",
                principalTable: "Stores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CreditsMaster_Customers_CustomerId",
                table: "CreditsMaster",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CreditsMaster_Status_StatusId",
                table: "CreditsMaster",
                column: "StatusId",
                principalTable: "Status",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CreditsMaster_Stores_StoreId",
                table: "CreditsMaster",
                column: "StoreId",
                principalTable: "Stores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Customers_Profiles_ProfileId",
                table: "Customers",
                column: "ProfileId",
                principalTable: "Profiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
