using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MessengerSY.Data.Migrations
{
    public partial class AddProp_LastMessageSendDate_ToChat : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LastMessageSendDate",
                table: "Chat",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastMessageSendDate",
                table: "Chat");
        }
    }
}
