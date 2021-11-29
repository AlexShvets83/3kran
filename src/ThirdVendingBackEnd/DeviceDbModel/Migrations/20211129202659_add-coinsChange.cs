using Microsoft.EntityFrameworkCore.Migrations;

namespace DeviceDbModel.Migrations
{
    public partial class addcoinsChange : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<float>(
                name: "coins_change",
                table: "device_sales",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "coins_change",
                table: "device_encashes",
                type: "real",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "coins_change",
                table: "device_sales");

            migrationBuilder.DropColumn(
                name: "coins_change",
                table: "device_encashes");
        }
    }
}
