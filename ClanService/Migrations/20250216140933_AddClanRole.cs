using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClanService.Migrations
{
    /// <inheritdoc />
    public partial class AddClanRole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ClanRole",
                table: "ClanMemberships",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClanRole",
                table: "ClanMemberships");
        }
    }
}
