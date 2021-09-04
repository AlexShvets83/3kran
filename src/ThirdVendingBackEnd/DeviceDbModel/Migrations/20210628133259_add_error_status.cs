using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace DeviceDbModel.Migrations
{
    public partial class add_error_status : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "device_error_status",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    device_id = table.Column<string>(type: "text", nullable: true),
                    message_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    total_sold = table.Column<float>(type: "real", nullable: false),
                    total_money = table.Column<float>(type: "real", nullable: false),
                    temperature = table.Column<float>(type: "real", nullable: true),
                    status = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_device_error_status", x => x.id);
                    table.ForeignKey(
                        name: "fk_device_error_status_devices_device_id",
                        column: x => x.device_id,
                        principalTable: "devices",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_device_error_status_device_id",
                table: "device_error_status",
                column: "device_id");

            migrationBuilder.CreateIndex(
                name: "ix_device_error_status_message_date",
                table: "device_error_status",
                column: "message_date");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "device_error_status");
        }
    }
}
