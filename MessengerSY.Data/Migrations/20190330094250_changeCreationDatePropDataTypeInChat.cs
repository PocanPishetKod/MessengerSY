using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MessengerSY.Data.Migrations
{
    public partial class changeCreationDatePropDataTypeInChat : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Message_Chat_ThisMessageLastInChatId",
                table: "Message");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreationDate",
                table: "Chat",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Message_Chat_ThisMessageLastInChatId",
                table: "Message",
                column: "ThisMessageLastInChatId",
                principalTable: "Chat",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Message_Chat_ThisMessageLastInChatId",
                table: "Message");

            migrationBuilder.AlterColumn<string>(
                name: "CreationDate",
                table: "Chat",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AddForeignKey(
                name: "FK_Message_Chat_ThisMessageLastInChatId",
                table: "Message",
                column: "ThisMessageLastInChatId",
                principalTable: "Chat",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
