using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClanService.Migrations
{
    /// <inheritdoc />
    public partial class SetClanSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "clan");

            migrationBuilder.RenameTable(
                name: "VoiceChannels",
                newName: "VoiceChannels",
                newSchema: "clan");

            migrationBuilder.RenameTable(
                name: "Users",
                newName: "Users",
                newSchema: "clan");

            migrationBuilder.RenameTable(
                name: "Clans",
                newName: "Clans",
                newSchema: "clan");

            migrationBuilder.RenameTable(
                name: "ClanMemberships",
                newName: "ClanMemberships",
                newSchema: "clan");

            migrationBuilder.RenameTable(
                name: "ClanInvitations",
                newName: "ClanInvitations",
                newSchema: "clan");

            migrationBuilder.RenameTable(
                name: "Channels",
                newName: "Channels",
                newSchema: "clan");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "VoiceChannels",
                schema: "clan",
                newName: "VoiceChannels");

            migrationBuilder.RenameTable(
                name: "Users",
                schema: "clan",
                newName: "Users");

            migrationBuilder.RenameTable(
                name: "Clans",
                schema: "clan",
                newName: "Clans");

            migrationBuilder.RenameTable(
                name: "ClanMemberships",
                schema: "clan",
                newName: "ClanMemberships");

            migrationBuilder.RenameTable(
                name: "ClanInvitations",
                schema: "clan",
                newName: "ClanInvitations");

            migrationBuilder.RenameTable(
                name: "Channels",
                schema: "clan",
                newName: "Channels");
        }
    }
}
