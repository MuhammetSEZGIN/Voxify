using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClanService.Migrations
{
    /// <inheritdoc />
    public partial class ClanRoleUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ClanRole",
                table: "ClanMemberships",
                newName: "Role");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Role",
                table: "ClanMemberships",
                newName: "ClanRole");
        }
    }
}
