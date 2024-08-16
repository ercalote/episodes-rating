using Microsoft.EntityFrameworkCore.Migrations;

namespace EpisodesRatingModel.Migrations
{
    public partial class AirDates : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Year",
                table: "Series",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AirDate",
                table: "Episodes",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Year",
                table: "Series");

            migrationBuilder.DropColumn(
                name: "AirDate",
                table: "Episodes");
        }
    }
}
