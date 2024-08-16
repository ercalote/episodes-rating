using Microsoft.EntityFrameworkCore.Migrations;

namespace EpisodesRatingModel.Migrations
{
    public partial class SeriesRatingVotes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Rating",
                table: "Series",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "VotesCount",
                table: "Series",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Rating",
                table: "Series");

            migrationBuilder.DropColumn(
                name: "VotesCount",
                table: "Series");
        }
    }
}
