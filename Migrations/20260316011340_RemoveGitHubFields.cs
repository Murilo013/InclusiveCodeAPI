using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InclusiveCode.API.Migrations
{
    /// <inheritdoc />
    public partial class RemoveGitHubFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "GitHubId",
                table: "Users",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsOAuth",
                table: "Users",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GitHubId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "IsOAuth",
                table: "Users");
        }
    }
}
