using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Anna.DataAccess.Migrations.Watchdog
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Guild",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "bigint unsigned", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    LoggingChannel = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    IsWatchdogEnabled = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: false),
                    IsAntiBotEnabled = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: true),
                    IsInviteKillerEnabled = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: true),
                    IsAutoKickEnabled = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: true),
                    IsFileRestrictionsEnabled = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: true),
                    MinimumAutoKickAge = table.Column<byte>(type: "tinyint unsigned", nullable: false, defaultValue: (byte)7)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Guild", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FileRestriction",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "bigint unsigned", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    RestrictorId = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    FileExtension = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: false),
                    GuildId = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    Created = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    Reason = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileRestriction", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FileRestriction_Guild_GuildId",
                        column: x => x.GuildId,
                        principalTable: "Guild",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Report",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "bigint unsigned", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ReportedId = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    ReporterId = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    GuildId = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    Description = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: false),
                    TimeOfReport = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Report", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Report_Guild_GuildId",
                        column: x => x.GuildId,
                        principalTable: "Guild",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FileRestriction_GuildId",
                table: "FileRestriction",
                column: "GuildId");

            migrationBuilder.CreateIndex(
                name: "IX_FileRestriction_Id",
                table: "FileRestriction",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Guild_Id",
                table: "Guild",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Report_GuildId",
                table: "Report",
                column: "GuildId");

            migrationBuilder.CreateIndex(
                name: "IX_Report_Id",
                table: "Report",
                column: "Id",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FileRestriction");

            migrationBuilder.DropTable(
                name: "Report");

            migrationBuilder.DropTable(
                name: "Guild");
        }
    }
}
