using Microsoft.EntityFrameworkCore.Migrations;

namespace Anna.DataAccess.Migrations.Watchdog
{
    public partial class AddInviteBlocker : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsInviteBlockerEnabled",
                table: "Guild",
                type: "tinyint(1)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsInviteBlockerEnabled",
                table: "Guild");
        }
    }
}
