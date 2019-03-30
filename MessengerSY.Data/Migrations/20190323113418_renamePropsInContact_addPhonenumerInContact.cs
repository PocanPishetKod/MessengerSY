using Microsoft.EntityFrameworkCore.Migrations;

namespace MessengerSY.Data.Migrations
{
    public partial class renamePropsInContact_addPhonenumerInContact : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Contact_UserProfile_FirstSideId",
                table: "Contact");

            migrationBuilder.DropForeignKey(
                name: "FK_Contact_UserProfile_SecondSideId",
                table: "Contact");

            migrationBuilder.DropIndex(
                name: "IX_Contact_FirstSideId",
                table: "Contact");

            migrationBuilder.DropColumn(
                name: "FirstSideId",
                table: "Contact");

            migrationBuilder.RenameColumn(
                name: "SecondSideId",
                table: "Contact",
                newName: "ContactOwnerId");

            migrationBuilder.RenameIndex(
                name: "IX_Contact_SecondSideId",
                table: "Contact",
                newName: "IX_Contact_ContactOwnerId");

            migrationBuilder.AddColumn<int>(
                name: "LinkedUserProfileId",
                table: "Contact",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "Contact",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Contact_LinkedUserProfileId",
                table: "Contact",
                column: "LinkedUserProfileId");

            migrationBuilder.AddForeignKey(
                name: "FK_Contact_UserProfile_ContactOwnerId",
                table: "Contact",
                column: "ContactOwnerId",
                principalTable: "UserProfile",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Contact_UserProfile_LinkedUserProfileId",
                table: "Contact",
                column: "LinkedUserProfileId",
                principalTable: "UserProfile",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Contact_UserProfile_ContactOwnerId",
                table: "Contact");

            migrationBuilder.DropForeignKey(
                name: "FK_Contact_UserProfile_LinkedUserProfileId",
                table: "Contact");

            migrationBuilder.DropIndex(
                name: "IX_Contact_LinkedUserProfileId",
                table: "Contact");

            migrationBuilder.DropColumn(
                name: "LinkedUserProfileId",
                table: "Contact");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "Contact");

            migrationBuilder.RenameColumn(
                name: "ContactOwnerId",
                table: "Contact",
                newName: "SecondSideId");

            migrationBuilder.RenameIndex(
                name: "IX_Contact_ContactOwnerId",
                table: "Contact",
                newName: "IX_Contact_SecondSideId");

            migrationBuilder.AddColumn<int>(
                name: "FirstSideId",
                table: "Contact",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Contact_FirstSideId",
                table: "Contact",
                column: "FirstSideId");

            migrationBuilder.AddForeignKey(
                name: "FK_Contact_UserProfile_FirstSideId",
                table: "Contact",
                column: "FirstSideId",
                principalTable: "UserProfile",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Contact_UserProfile_SecondSideId",
                table: "Contact",
                column: "SecondSideId",
                principalTable: "UserProfile",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
