using Microsoft.EntityFrameworkCore.Migrations;

namespace DeviceDbModel.Migrations
{
    public partial class updatestatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "alert",
                table: "device_last_status",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "change_status",
                table: "device_last_status",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "current_change",
                table: "device_last_status",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "nfc_card",
                table: "device_last_status",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "total_change",
                table: "device_last_status",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "total_rest",
                table: "device_last_status",
                type: "real",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "alert",
                table: "device_last_status");

            migrationBuilder.DropColumn(
                name: "change_status",
                table: "device_last_status");

            migrationBuilder.DropColumn(
                name: "current_change",
                table: "device_last_status");

            migrationBuilder.DropColumn(
                name: "nfc_card",
                table: "device_last_status");

            migrationBuilder.DropColumn(
                name: "total_change",
                table: "device_last_status");

            migrationBuilder.DropColumn(
                name: "total_rest",
                table: "device_last_status");
        }
    }
}
