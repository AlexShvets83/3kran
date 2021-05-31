using Microsoft.EntityFrameworkCore.Migrations;

namespace DeviceDbModel.Migrations
{
    public partial class add_topicType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "topic_type",
                table: "device_settings",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "ix_device_settings_topic_type",
                table: "device_settings",
                column: "topic_type");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_device_settings_topic_type",
                table: "device_settings");

            migrationBuilder.DropColumn(
                name: "topic_type",
                table: "device_settings");
        }
    }
}
