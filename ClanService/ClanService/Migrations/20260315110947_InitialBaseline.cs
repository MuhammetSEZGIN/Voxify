using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClanService.Migrations
{
    /// <inheritdoc />
    public partial class InitialBaseline : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "clan");

            migrationBuilder.CreateTable(
                name: "Clans",
                schema: "clan",
                columns: table => new
                {
                    ClanId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ImagePath = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    IsPublic = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clans", x => x.ClanId);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                schema: "clan",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Username = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    AvatarUrl = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Channels",
                schema: "clan",
                columns: table => new
                {
                    ChannelId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ClanId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Channels", x => x.ChannelId);
                    table.ForeignKey(
                        name: "FK_Channels_Clans_ClanId",
                        column: x => x.ClanId,
                        principalSchema: "clan",
                        principalTable: "Clans",
                        principalColumn: "ClanId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ClanInvitations",
                schema: "clan",
                columns: table => new
                {
                    InviteId = table.Column<Guid>(type: "uuid", nullable: false),
                    ClanId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    InviteCode = table.Column<string>(type: "text", nullable: false),
                    MaxUses = table.Column<int>(type: "integer", nullable: false),
                    UsedCount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClanInvitations", x => x.InviteId);
                    table.ForeignKey(
                        name: "FK_ClanInvitations_Clans_ClanId",
                        column: x => x.ClanId,
                        principalSchema: "clan",
                        principalTable: "Clans",
                        principalColumn: "ClanId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VoiceChannels",
                schema: "clan",
                columns: table => new
                {
                    VoiceChannelId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ClanId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    MaxParticipants = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VoiceChannels", x => x.VoiceChannelId);
                    table.ForeignKey(
                        name: "FK_VoiceChannels_Clans_ClanId",
                        column: x => x.ClanId,
                        principalSchema: "clan",
                        principalTable: "Clans",
                        principalColumn: "ClanId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ClanMemberships",
                schema: "clan",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ClanId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    Role = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClanMemberships", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClanMemberships_Clans_ClanId",
                        column: x => x.ClanId,
                        principalSchema: "clan",
                        principalTable: "Clans",
                        principalColumn: "ClanId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClanMemberships_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "clan",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Channels_ClanId",
                schema: "clan",
                table: "Channels",
                column: "ClanId");

            migrationBuilder.CreateIndex(
                name: "IX_ClanInvitations_ClanId",
                schema: "clan",
                table: "ClanInvitations",
                column: "ClanId");

            migrationBuilder.CreateIndex(
                name: "IX_ClanMemberships_ClanId",
                schema: "clan",
                table: "ClanMemberships",
                column: "ClanId");

            migrationBuilder.CreateIndex(
                name: "IX_ClanMemberships_UserId",
                schema: "clan",
                table: "ClanMemberships",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Clans_Description",
                schema: "clan",
                table: "Clans",
                column: "Description");

            migrationBuilder.CreateIndex(
                name: "IX_Clans_Name",
                schema: "clan",
                table: "Clans",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_VoiceChannels_ClanId",
                schema: "clan",
                table: "VoiceChannels",
                column: "ClanId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Channels",
                schema: "clan");

            migrationBuilder.DropTable(
                name: "ClanInvitations",
                schema: "clan");

            migrationBuilder.DropTable(
                name: "ClanMemberships",
                schema: "clan");

            migrationBuilder.DropTable(
                name: "VoiceChannels",
                schema: "clan");

            migrationBuilder.DropTable(
                name: "Users",
                schema: "clan");

            migrationBuilder.DropTable(
                name: "Clans",
                schema: "clan");
        }
    }
}
