using Microsoft.EntityFrameworkCore.Migrations;

namespace DeviceDbModel.Migrations
{
    public partial class addrest : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<float>(
                name: "rest",
                table: "device_sales",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "rest",
                table: "device_encashes",
                type: "real",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "rest",
                table: "device_sales");

            migrationBuilder.DropColumn(
                name: "rest",
                table: "device_encashes");
        }
    }
}
