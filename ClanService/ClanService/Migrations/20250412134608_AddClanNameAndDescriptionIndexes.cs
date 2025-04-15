using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClanService.Migrations
{
    /// <inheritdoc />
    public partial class AddClanNameAndDescriptionIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Clans",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsPublic",
                table: "Clans",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Clans_Description",
                table: "Clans",
                column: "Description");

            migrationBuilder.CreateIndex(
                name: "IX_Clans_Name",
                table: "Clans",
                column: "Name");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Clans_Description",
                table: "Clans");

            migrationBuilder.DropIndex(
                name: "IX_Clans_Name",
                table: "Clans");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Clans");

            migrationBuilder.DropColumn(
                name: "IsPublic",
                table: "Clans");
        }
    }
}
