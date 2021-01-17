using Microsoft.EntityFrameworkCore.Migrations;

namespace WebScraperMF.Migrations
{
    public partial class addCurrencyToDB : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "productPriceCurrency",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "productPriceCurrency",
                table: "Products");
        }
    }
}
