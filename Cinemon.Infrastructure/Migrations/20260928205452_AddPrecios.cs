using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cinemon.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPrecios : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Precios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Formato = table.Column<int>(type: "int", nullable: false),
                    TipoSala = table.Column<int>(type: "int", nullable: false),
                    Valor = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Precios", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Precios_Formato_TipoSala",
                table: "Precios",
                columns: new[] { "Formato", "TipoSala" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Precios");
        }
    }
}
