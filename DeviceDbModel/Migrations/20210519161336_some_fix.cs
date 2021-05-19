using Microsoft.EntityFrameworkCore.Migrations;

namespace DeviceDbModel.Migrations
{
    public partial class some_fix : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "mesage_date",
                table: "device_sales",
                newName: "message_date");

            migrationBuilder.RenameIndex(
                name: "ix_device_sales_mesage_date",
                table: "device_sales",
                newName: "ix_device_sales_message_date");

            migrationBuilder.RenameColumn(
                name: "mesage_date",
                table: "device_last_status",
                newName: "message_date");

            migrationBuilder.RenameIndex(
                name: "ix_device_last_status_mesage_date",
                table: "device_last_status",
                newName: "ix_device_last_status_message_date");

            migrationBuilder.RenameColumn(
                name: "mesage_date",
                table: "device_encashes",
                newName: "message_date");

            migrationBuilder.RenameIndex(
                name: "ix_device_encashes_mesage_date",
                table: "device_encashes",
                newName: "ix_device_encashes_message_date");

            migrationBuilder.RenameColumn(
                name: "mesage_date",
                table: "device_alerts",
                newName: "message_date");

            migrationBuilder.RenameIndex(
                name: "ix_device_alerts_mesage_date",
                table: "device_alerts",
                newName: "ix_device_alerts_message_date");

            migrationBuilder.AlterColumn<float>(
                name: "total_sold",
                table: "device_last_status",
                type: "real",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "double precision");

            migrationBuilder.AlterColumn<float>(
                name: "total_money",
                table: "device_last_status",
                type: "real",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "double precision");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "message_date",
                table: "device_sales",
                newName: "mesage_date");

            migrationBuilder.RenameIndex(
                name: "ix_device_sales_message_date",
                table: "device_sales",
                newName: "ix_device_sales_mesage_date");

            migrationBuilder.RenameColumn(
                name: "message_date",
                table: "device_last_status",
                newName: "mesage_date");

            migrationBuilder.RenameIndex(
                name: "ix_device_last_status_message_date",
                table: "device_last_status",
                newName: "ix_device_last_status_mesage_date");

            migrationBuilder.RenameColumn(
                name: "message_date",
                table: "device_encashes",
                newName: "mesage_date");

            migrationBuilder.RenameIndex(
                name: "ix_device_encashes_message_date",
                table: "device_encashes",
                newName: "ix_device_encashes_mesage_date");

            migrationBuilder.RenameColumn(
                name: "message_date",
                table: "device_alerts",
                newName: "mesage_date");

            migrationBuilder.RenameIndex(
                name: "ix_device_alerts_message_date",
                table: "device_alerts",
                newName: "ix_device_alerts_mesage_date");

            migrationBuilder.AlterColumn<double>(
                name: "total_sold",
                table: "device_last_status",
                type: "double precision",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "real");

            migrationBuilder.AlterColumn<double>(
                name: "total_money",
                table: "device_last_status",
                type: "double precision",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "real");
        }
    }
}
