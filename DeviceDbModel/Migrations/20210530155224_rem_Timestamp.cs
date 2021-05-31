using Microsoft.EntityFrameworkCore.Migrations;

namespace DeviceDbModel.Migrations
{
    public partial class rem_Timestamp : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "timestamp",
                table: "device_sales");

            migrationBuilder.DropColumn(
                name: "timestamp",
                table: "device_last_status");

            migrationBuilder.DropColumn(
                name: "timestamp",
                table: "device_encashes");

            migrationBuilder.DropColumn(
                name: "timestamp",
                table: "device_alerts");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "timestamp",
                table: "device_sales",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "timestamp",
                table: "device_last_status",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "timestamp",
                table: "device_encashes",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "timestamp",
                table: "device_alerts",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);
        }
    }
}
