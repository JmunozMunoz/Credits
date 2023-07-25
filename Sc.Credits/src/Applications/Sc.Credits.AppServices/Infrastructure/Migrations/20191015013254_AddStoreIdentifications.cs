using Microsoft.EntityFrameworkCore.Migrations;

namespace Sc.Credits.Applications.AppServices.Infrastructure.Migrations
{
    public partial class AddStoreIdentifications : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateSequence(
                name: "StoreIdentificationsSeq",
                incrementBy: 10);

            migrationBuilder.CreateTable(
                name: "StoreIdentifications",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    StoreId = table.Column<string>(maxLength: 50, nullable: false),
                    ScCode = table.Column<string>(maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoreIdentifications", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StoreIdentifications");

            migrationBuilder.DropSequence(
                name: "StoreIdentificationsSeq");
        }
    }
}
