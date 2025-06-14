using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Proyecto_FinalProgra1.Migrations
{
    /// <inheritdoc />
    public partial class AddSentimentToReview : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "SentimentPositive",
                table: "Reviews",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "SentimentProbability",
                table: "Reviews",
                type: "real",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SentimentPositive",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "SentimentProbability",
                table: "Reviews");
        }
    }
}
