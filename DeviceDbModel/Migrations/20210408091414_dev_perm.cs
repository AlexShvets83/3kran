using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DeviceDbModel.Migrations
{
    public partial class dev_perm : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "country_id",
                table: "AspNetUsers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "invite_code",
                table: "AspNetUsers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "owner_id",
                table: "AspNetUsers",
                type: "text",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "countries",
                columns: table => new
                {
                    id = table.Column<string>(type: "text", nullable: false),
                    name = table.Column<string>(type: "text", nullable: true),
                    alpha2code = table.Column<string>(type: "text", nullable: true),
                    alpha3code = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_countries", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "devices",
                columns: table => new
                {
                    id = table.Column<string>(type: "text", nullable: false),
                    device_id = table.Column<string>(type: "text", nullable: true),
                    owner_id = table.Column<string>(type: "text", nullable: true),
                    address = table.Column<string>(type: "text", nullable: true),
                    time_zone = table.Column<int>(type: "integer", nullable: false),
                    currency = table.Column<string>(type: "text", nullable: true),
                    phone = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_devices", x => x.id);
                    table.ForeignKey(
                        name: "fk_devices_users_user_id",
                        column: x => x.owner_id,
                        principalTable: "AspNetUsers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "invite_registrations",
                columns: table => new
                {
                    id = table.Column<string>(type: "text", nullable: false),
                    user_id = table.Column<string>(type: "text", nullable: true),
                    email = table.Column<string>(type: "text", nullable: true),
                    invite_code = table.Column<string>(type: "text", nullable: true),
                    expiration_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_invite_registrations", x => x.id);
                    table.ForeignKey(
                        name: "fk_invite_registrations_users_user_id",
                        column: x => x.user_id,
                        principalTable: "AspNetUsers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_device_permissions",
                columns: table => new
                {
                    id = table.Column<string>(type: "text", nullable: false),
                    user_id = table.Column<string>(type: "text", nullable: true),
                    device_id = table.Column<string>(type: "text", nullable: true),
                    commerce_visible = table.Column<bool>(type: "boolean", nullable: false),
                    tech_editable = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_device_permissions", x => x.id);
                    table.ForeignKey(
                        name: "fk_user_device_permissions_devices_device_id",
                        column: x => x.device_id,
                        principalTable: "devices",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_user_device_permissions_users_user_id",
                        column: x => x.user_id,
                        principalTable: "AspNetUsers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_asp_net_users_country_id",
                table: "AspNetUsers",
                column: "country_id");

            migrationBuilder.CreateIndex(
                name: "ix_asp_net_users_owner_id",
                table: "AspNetUsers",
                column: "owner_id");

            migrationBuilder.CreateIndex(
                name: "ix_devices_address",
                table: "devices",
                column: "address");

            migrationBuilder.CreateIndex(
                name: "ix_devices_device_id",
                table: "devices",
                column: "device_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_devices_owner_id",
                table: "devices",
                column: "owner_id");

            migrationBuilder.CreateIndex(
                name: "ix_invite_registrations_user_id",
                table: "invite_registrations",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_device_permissions_device_id",
                table: "user_device_permissions",
                column: "device_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_device_permissions_user_id",
                table: "user_device_permissions",
                column: "user_id");

            migrationBuilder.AddForeignKey(
                name: "fk_asp_net_users_asp_net_users_master_id",
                table: "AspNetUsers",
                column: "owner_id",
                principalTable: "AspNetUsers",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_asp_net_users_countries_country_id",
                table: "AspNetUsers",
                column: "country_id",
                principalTable: "countries",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_asp_net_users_asp_net_users_master_id",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "fk_asp_net_users_countries_country_id",
                table: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "countries");

            migrationBuilder.DropTable(
                name: "invite_registrations");

            migrationBuilder.DropTable(
                name: "user_device_permissions");

            migrationBuilder.DropTable(
                name: "devices");

            migrationBuilder.DropIndex(
                name: "ix_asp_net_users_country_id",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "ix_asp_net_users_owner_id",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "country_id",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "invite_code",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "owner_id",
                table: "AspNetUsers");
        }
    }
}
