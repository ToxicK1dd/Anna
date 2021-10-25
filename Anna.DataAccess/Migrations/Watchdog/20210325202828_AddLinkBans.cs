using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Anna.DataAccess.Migrations.Watchdog
{
    public partial class AddLinkBans : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsBannedLinksEnabled",
                table: "Guild",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: true);

            migrationBuilder.CreateTable(
                name: "LinkBan",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "bigint unsigned", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    CreatedBy = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    Url = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: false),
                    GuildId = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    Created = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    Reason = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LinkBan", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LinkBan_Guild_GuildId",
                        column: x => x.GuildId,
                        principalTable: "Guild",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LinkBan_GuildId",
                table: "LinkBan",
                column: "GuildId");

            migrationBuilder.CreateIndex(
                name: "IX_LinkBan_Id",
                table: "LinkBan",
                column: "Id",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LinkBan");

            migrationBuilder.DropColumn(
                name: "IsBannedLinksEnabled",
                table: "Guild");
        }
    }
}
