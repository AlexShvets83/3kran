using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace DeviceDbModel.Migrations
{
    public partial class add_appSettings : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "app_settings",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_log_depth = table.Column<int>(type: "integer", nullable: false),
                    support_board1 = table.Column<bool>(type: "boolean", nullable: false),
                    support_board2 = table.Column<bool>(type: "boolean", nullable: false),
                    support_board3 = table.Column<bool>(type: "boolean", nullable: false),
                    file_max_upload_lenght = table.Column<long>(type: "bigint", nullable: false),
                    users_max_downloads = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_app_settings", x => x.id);
                });

            migrationBuilder.InsertData(
                table: "app_settings",
                columns: new[] { "id", "file_max_upload_lenght", "support_board1", "support_board2", "support_board3", "user_log_depth", "users_max_downloads" },
                values: new object[] { 1, 300L, true, true, true, 30, 5 });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "app_settings");
        }
    }
}
