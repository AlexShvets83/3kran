using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DeviceDbModel.Migrations
{
    public partial class add_main_data_table : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "device_id",
                table: "devices",
                newName: "imei");

            migrationBuilder.RenameIndex(
                name: "ix_devices_device_id",
                table: "devices",
                newName: "ix_devices_imei");

            migrationBuilder.CreateTable(
                name: "device_alerts",
                columns: table => new
                {
                    id = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    device_id = table.Column<string>(type: "text", nullable: true),
                    received_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    mesage_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    timestamp = table.Column<double>(type: "double precision", nullable: false),
                    code_type = table.Column<int>(type: "integer", nullable: false),
                    message = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_device_alerts", x => x.id);
                    table.ForeignKey(
                        name: "fk_device_alerts_devices_device_id",
                        column: x => x.device_id,
                        principalTable: "devices",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "device_encashes",
                columns: table => new
                {
                    id = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    device_id = table.Column<string>(type: "text", nullable: true),
                    received_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    mesage_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    timestamp = table.Column<double>(type: "double precision", nullable: false),
                    amount_coin = table.Column<float>(type: "real", nullable: false),
                    amount_bill = table.Column<float>(type: "real", nullable: false),
                    amount = table.Column<float>(type: "real", nullable: false),
                    coins = table.Column<string>(type: "text", nullable: true),
                    bills = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_device_encashes", x => x.id);
                    table.ForeignKey(
                        name: "fk_device_encashes_devices_device_id",
                        column: x => x.device_id,
                        principalTable: "devices",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "device_last_status",
                columns: table => new
                {
                    id = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    device_id = table.Column<string>(type: "text", nullable: true),
                    received_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    mesage_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    timestamp = table.Column<double>(type: "double precision", nullable: false),
                    total_sold = table.Column<double>(type: "double precision", nullable: false),
                    total_money = table.Column<double>(type: "double precision", nullable: false),
                    temperature = table.Column<float>(type: "real", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_device_last_status", x => x.id);
                    table.ForeignKey(
                        name: "fk_device_last_status_devices_device_id",
                        column: x => x.device_id,
                        principalTable: "devices",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "device_sales",
                columns: table => new
                {
                    id = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    device_id = table.Column<string>(type: "text", nullable: true),
                    received_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    mesage_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    timestamp = table.Column<double>(type: "double precision", nullable: false),
                    payment_type = table.Column<int>(type: "integer", nullable: false),
                    quantity = table.Column<float>(type: "real", nullable: false),
                    price = table.Column<float>(type: "real", nullable: false),
                    amount = table.Column<float>(type: "real", nullable: false),
                    coins = table.Column<string>(type: "text", nullable: true),
                    bills = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_device_sales", x => x.id);
                    table.ForeignKey(
                        name: "fk_device_sales_devices_device_id",
                        column: x => x.device_id,
                        principalTable: "devices",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_device_alerts_device_id",
                table: "device_alerts",
                column: "device_id");

            migrationBuilder.CreateIndex(
                name: "ix_device_alerts_mesage_date",
                table: "device_alerts",
                column: "mesage_date");

            migrationBuilder.CreateIndex(
                name: "ix_device_alerts_received_date",
                table: "device_alerts",
                column: "received_date");

            migrationBuilder.CreateIndex(
                name: "ix_device_encashes_device_id",
                table: "device_encashes",
                column: "device_id");

            migrationBuilder.CreateIndex(
                name: "ix_device_encashes_mesage_date",
                table: "device_encashes",
                column: "mesage_date");

            migrationBuilder.CreateIndex(
                name: "ix_device_encashes_received_date",
                table: "device_encashes",
                column: "received_date");

            migrationBuilder.CreateIndex(
                name: "ix_device_last_status_device_id",
                table: "device_last_status",
                column: "device_id");

            migrationBuilder.CreateIndex(
                name: "ix_device_last_status_mesage_date",
                table: "device_last_status",
                column: "mesage_date");

            migrationBuilder.CreateIndex(
                name: "ix_device_last_status_received_date",
                table: "device_last_status",
                column: "received_date");

            migrationBuilder.CreateIndex(
                name: "ix_device_sales_device_id",
                table: "device_sales",
                column: "device_id");

            migrationBuilder.CreateIndex(
                name: "ix_device_sales_mesage_date",
                table: "device_sales",
                column: "mesage_date");

            migrationBuilder.CreateIndex(
                name: "ix_device_sales_received_date",
                table: "device_sales",
                column: "received_date");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "device_alerts");

            migrationBuilder.DropTable(
                name: "device_encashes");

            migrationBuilder.DropTable(
                name: "device_last_status");

            migrationBuilder.DropTable(
                name: "device_sales");

            migrationBuilder.RenameColumn(
                name: "imei",
                table: "devices",
                newName: "device_id");

            migrationBuilder.RenameIndex(
                name: "ix_devices_imei",
                table: "devices",
                newName: "ix_devices_device_id");
        }
    }
}
