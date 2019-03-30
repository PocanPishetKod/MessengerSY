using Microsoft.EntityFrameworkCore.Migrations;

namespace MessengerSY.Data.Migrations
{
    public partial class AddProperty_ContactName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ContactName",
                table: "Contact",
                maxLength: 50,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContactName",
                table: "Contact");
        }
    }
}
