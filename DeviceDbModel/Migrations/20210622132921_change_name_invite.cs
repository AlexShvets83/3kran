using Microsoft.EntityFrameworkCore.Migrations;

namespace DeviceDbModel.Migrations
{
    public partial class change_name_invite : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_devices_users_user_id",
                table: "devices");

            migrationBuilder.RenameColumn(
                name: "user_id",
                table: "invite_registrations",
                newName: "owner_id");

            migrationBuilder.RenameIndex(
                name: "ix_invite_registrations_user_id",
                table: "invite_registrations",
                newName: "ix_invite_registrations_owner_id");

            migrationBuilder.AddForeignKey(
                name: "fk_devices_users_user_id",
                table: "devices",
                column: "owner_id",
                principalTable: "AspNetUsers",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_devices_users_user_id",
                table: "devices");

            migrationBuilder.RenameColumn(
                name: "owner_id",
                table: "invite_registrations",
                newName: "user_id");

            migrationBuilder.RenameIndex(
                name: "ix_invite_registrations_owner_id",
                table: "invite_registrations",
                newName: "ix_invite_registrations_user_id");

            migrationBuilder.AddForeignKey(
                name: "fk_devices_users_user_id",
                table: "devices",
                column: "owner_id",
                principalTable: "AspNetUsers",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
