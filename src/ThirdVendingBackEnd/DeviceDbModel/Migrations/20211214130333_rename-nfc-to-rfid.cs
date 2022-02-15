using Microsoft.EntityFrameworkCore.Migrations;

namespace DeviceDbModel.Migrations
{
    public partial class renamenfctorfid : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "nfc_card",
                table: "device_sales",
                newName: "rfid_card");

            migrationBuilder.RenameColumn(
                name: "nfc_card",
                table: "device_encashes",
                newName: "rfid_card");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "rfid_card",
                table: "device_sales",
                newName: "nfc_card");

            migrationBuilder.RenameColumn(
                name: "rfid_card",
                table: "device_encashes",
                newName: "nfc_card");
        }
    }
}
