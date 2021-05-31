using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DeviceDbModel.Migrations
{
    public partial class rem_ReceivedDate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_device_sales_received_date",
                table: "device_sales");

            migrationBuilder.DropIndex(
                name: "ix_device_last_status_received_date",
                table: "device_last_status");

            migrationBuilder.DropIndex(
                name: "ix_device_encashes_received_date",
                table: "device_encashes");

            migrationBuilder.DropIndex(
                name: "ix_device_alerts_received_date",
                table: "device_alerts");

            migrationBuilder.DropColumn(
                name: "received_date",
                table: "device_settings");

            migrationBuilder.DropColumn(
                name: "received_date",
                table: "device_sales");

            migrationBuilder.DropColumn(
                name: "received_date",
                table: "device_last_status");

            migrationBuilder.DropColumn(
                name: "received_date",
                table: "device_infos");

            migrationBuilder.DropColumn(
                name: "received_date",
                table: "device_encashes");

            migrationBuilder.DropColumn(
                name: "received_date",
                table: "device_alerts");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "received_date",
                table: "device_settings",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "received_date",
                table: "device_sales",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "received_date",
                table: "device_last_status",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "received_date",
                table: "device_infos",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "received_date",
                table: "device_encashes",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "received_date",
                table: "device_alerts",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "ix_device_sales_received_date",
                table: "device_sales",
                column: "received_date");

            migrationBuilder.CreateIndex(
                name: "ix_device_last_status_received_date",
                table: "device_last_status",
                column: "received_date");

            migrationBuilder.CreateIndex(
                name: "ix_device_encashes_received_date",
                table: "device_encashes",
                column: "received_date");

            migrationBuilder.CreateIndex(
                name: "ix_device_alerts_received_date",
                table: "device_alerts",
                column: "received_date");
        }
    }
}
