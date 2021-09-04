using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace DeviceDbModel.Migrations
{
    public partial class fix_info : Migration
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
                                         value = table.Column<string>(type: "real", nullable: true)
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
            //migrationBuilder.AlterColumn<float>(
            //    name: "value",
            //    table: "device_infos",
            //    type: "real",
            //    nullable: false,
            //    defaultValue: 0f,
            //    oldClrType: typeof(string),
            //    oldType: "text",
            //    oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
          migrationBuilder.DropTable(
                                     name: "device_infos");
            //migrationBuilder.AlterColumn<string>(
            //    name: "value",
            //    table: "device_infos",
            //    type: "text",
            //    nullable: true,
            //    oldClrType: typeof(float),
            //    oldType: "real");
        }
    }
}
