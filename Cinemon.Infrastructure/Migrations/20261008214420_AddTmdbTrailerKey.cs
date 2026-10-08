using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cinemon.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTmdbTrailerKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TmdbTrailerKey",
                table: "Peliculas",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TmdbTrailerKey",
                table: "Peliculas");
        }
    }
}
