using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Sc.Credits.Applications.AppServices.infrastructure.migrations
{
    public partial class AddStoresCategories : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StoresCategories",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 200, nullable: false),
                    RegularFeesNumber = table.Column<int>(nullable: false),
                    MaximumFeesNumber = table.Column<int>(nullable: false),
                    MinimumFeeValue = table.Column<decimal>(type: "decimal(26, 6)", nullable: false),
                    MaximumCreditValue = table.Column<decimal>(type: "decimal(26, 6)", nullable: false),
                    FeeCutoffValue = table.Column<decimal>(type: "decimal(26, 6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoresCategories", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StoresCategories");
        }
    }
}