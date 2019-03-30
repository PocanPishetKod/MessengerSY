using Microsoft.EntityFrameworkCore.Migrations;

namespace MessengerSY.Data.Migrations
{
    public partial class AddProp_LastMessage_ToChat : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ThisMessageLastInChatId",
                table: "Message",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Message_ThisMessageLastInChatId",
                table: "Message",
                column: "ThisMessageLastInChatId",
                unique: true,
                filter: "[ThisMessageLastInChatId] IS NOT NULL");

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

            migrationBuilder.DropIndex(
                name: "IX_Message_ThisMessageLastInChatId",
                table: "Message");

            migrationBuilder.DropColumn(
                name: "ThisMessageLastInChatId",
                table: "Message");
        }
    }
}
