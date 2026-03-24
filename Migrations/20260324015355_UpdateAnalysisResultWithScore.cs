using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InclusiveCode.API.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAnalysisResultWithScore : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Score",
                table: "AnalysisResults",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ScoreLabel",
                table: "AnalysisResults",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Score",
                table: "AnalysisResults");

            migrationBuilder.DropColumn(
                name: "ScoreLabel",
                table: "AnalysisResults");
        }
    }
}
