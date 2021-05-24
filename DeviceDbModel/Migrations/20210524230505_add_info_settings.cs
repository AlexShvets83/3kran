using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace DeviceDbModel.Migrations
{
    public partial class add_info_settings : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "device_infos",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    device_id = table.Column<string>(type: "text", nullable: true),
                    received_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    message_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    name = table.Column<string>(type: "text", nullable: true),
                    value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_device_infos", x => x.id);
                    table.ForeignKey(
                        name: "fk_device_infos_devices_device_id",
                        column: x => x.device_id,
                        principalTable: "devices",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "device_settings",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    device_id = table.Column<string>(type: "text", nullable: true),
                    received_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    message_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    topic = table.Column<string>(type: "text", nullable: true),
                    payload = table.Column<string>(type: "text", nullable: true),
                    md5 = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_device_settings", x => x.id);
                    table.ForeignKey(
                        name: "fk_device_settings_devices_device_id",
                        column: x => x.device_id,
                        principalTable: "devices",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_device_infos_device_id",
                table: "device_infos",
                column: "device_id");

            migrationBuilder.CreateIndex(
                name: "ix_device_infos_device_id_name",
                table: "device_infos",
                columns: new[] { "device_id", "name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_device_infos_message_date",
                table: "device_infos",
                column: "message_date");

            migrationBuilder.CreateIndex(
                name: "ix_device_settings_device_id",
                table: "device_settings",
                column: "device_id");

            migrationBuilder.CreateIndex(
                name: "ix_device_settings_device_id_message_date_md5",
                table: "device_settings",
                columns: new[] { "device_id", "message_date", "md5" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_device_settings_message_date",
                table: "device_settings",
                column: "message_date");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "device_infos");

            migrationBuilder.DropTable(
                name: "device_settings");
        }
    }
}
