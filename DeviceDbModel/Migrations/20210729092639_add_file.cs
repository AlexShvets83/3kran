using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DeviceDbModel.Migrations
{
    public partial class add_file : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "files",
                columns: table => new
                {
                    id = table.Column<string>(type: "text", nullable: false),
                    file_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    path = table.Column<string>(type: "text", nullable: true),
                    name = table.Column<string>(type: "text", nullable: true),
                    description = table.Column<string>(type: "text", nullable: true),
                    size = table.Column<long>(type: "bigint", nullable: false),
                    file_type = table.Column<string>(type: "text", nullable: true),
                    visible = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_files", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_files_visible",
                table: "files",
                column: "visible");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "files");
        }
    }
}
