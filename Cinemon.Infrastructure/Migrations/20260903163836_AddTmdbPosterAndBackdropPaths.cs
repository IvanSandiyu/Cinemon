using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cinemon.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTmdbPosterAndBackdropPaths : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TmdbBackdropPath",
                table: "Peliculas",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TmdbPosterPath",
                table: "Peliculas",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TmdbBackdropPath",
                table: "Peliculas");

            migrationBuilder.DropColumn(
                name: "TmdbPosterPath",
                table: "Peliculas");
        }
    }
}
