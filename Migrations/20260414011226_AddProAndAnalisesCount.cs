using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InclusiveCode.API.Migrations
{
    /// <inheritdoc />
    public partial class AddProAndAnalisesCount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AnalisesCount",
                table: "Users",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "Pro",
                table: "Users",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AnalisesCount",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Pro",
                table: "Users");
        }
    }
}
