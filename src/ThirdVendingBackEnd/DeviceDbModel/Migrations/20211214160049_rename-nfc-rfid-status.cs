using Microsoft.EntityFrameworkCore.Migrations;

namespace DeviceDbModel.Migrations
{
    public partial class renamenfcrfidstatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "nfc_card",
                table: "device_last_status",
                newName: "rfid_card");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "rfid_card",
                table: "device_last_status",
                newName: "nfc_card");
        }
    }
}
