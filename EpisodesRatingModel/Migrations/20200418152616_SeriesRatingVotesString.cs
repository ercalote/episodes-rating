using Microsoft.EntityFrameworkCore.Migrations;

namespace EpisodesRatingModel.Migrations
{
    public partial class SeriesRatingVotesString : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "VotesCount",
                table: "Series",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Rating",
                table: "Series",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float",
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "VotesCount",
                table: "Series",
                type: "int",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "Rating",
                table: "Series",
                type: "float",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}
