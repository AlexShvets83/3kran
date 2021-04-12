using Microsoft.EntityFrameworkCore.Migrations;

namespace DeviceDbModel.Migrations
{
    public partial class stage2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_asp_net_users_asp_net_users_master_id",
                table: "AspNetUsers");

            migrationBuilder.AddForeignKey(
                name: "fk_asp_net_users_asp_net_users_owner_id",
                table: "AspNetUsers",
                column: "owner_id",
                principalTable: "AspNetUsers",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_asp_net_users_asp_net_users_owner_id",
                table: "AspNetUsers");

            migrationBuilder.AddForeignKey(
                name: "fk_asp_net_users_asp_net_users_master_id",
                table: "AspNetUsers",
                column: "owner_id",
                principalTable: "AspNetUsers",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
