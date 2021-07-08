using Microsoft.EntityFrameworkCore.Migrations;

namespace DeviceDbModel.Migrations
{
    public partial class AddCommerceVisible : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "commerce_visible",
                table: "AspNetUsers",
                type: "boolean",
                nullable: false,
                defaultValueSql: "true");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "commerce_visible",
                table: "AspNetUsers");
        }
    }
}
